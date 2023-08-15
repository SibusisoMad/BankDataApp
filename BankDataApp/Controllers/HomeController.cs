using BankDataApp.Interfaces;
using BankDataApp.Models;
using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using NuGet.Common;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Threading;

namespace BankDataApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IPaymentRepository _paymentRepository;



        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment webHostEnvironment, IPaymentRepository paymentRepository)
        {
            _logger = logger;
            _webHostEnvironment= webHostEnvironment;
            _paymentRepository= paymentRepository;
        }

        public IActionResult Index()
        {
            ViewBag.mssg = TempData["mssg"] as string;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }


        public async Task<ActionResult> _ViewImportsAsync(string id)
        {
            string acc = "";
            if (id == null) acc = "321456789374";
            else acc = id;
            var masters = await _paymentRepository.GetMasterDetails();
            var details = await _paymentRepository.GetDetails(acc);

            var model = new MasterDetail
            {
                Masters = masters,
                Details = details
            };

            return PartialView("_ViewImports", model);
        }

        public async Task<ActionResult> _ViewReport()
        {
            var report = await _paymentRepository.GetReport();
            return PartialView("_ViewReport",report);
        }

        public async Task<ActionResult> _UploadCSV(IFormFile postedFile, CancellationToken cancellationToken)
        {
            try
            {
                Payments payments = new Payments();
                int id = 0;
                string webRootPath = _webHostEnvironment.WebRootPath;
                string filePath = string.Empty;
                if (postedFile != null)
                {
                    string path = Path.Combine(webRootPath, "Uploads");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    filePath = path + "\\" + Path.GetFileName(postedFile.FileName);
                    if (System.IO.File.Exists(filePath))
                    {
                        TempData["mssg"] = "File Uploaded Already";
                        return RedirectToAction("Index", "Home");
                    }
                    string extension = Path.GetExtension(postedFile.FileName);
                    if (extension != ".csv")
                    {
                        TempData["mssg"] = "Only CSVs are allowed";
                        return RedirectToAction("Index", "Home");
                    }
                    await Task.Delay(10000, cancellationToken); // Simulate a delay during the upload
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return RedirectToAction("Index", "Home");
                    }

                    using (Stream fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await postedFile.CopyToAsync(fileStream);
                    }

                    //Read the contents of CSV file.
                    DataTable dz = new DataTable();
                    using (var csv = new CsvReader(System.IO.File.OpenText(filePath), System.Globalization.CultureInfo.CurrentCulture))
                    {
                        // Do any configuration to `CsvReader` before creating CsvDataReader.
                        using (var dr = new CsvDataReader(csv))
                        {
                            //var dt = new DataTable();
                            dz.Load(dr);
                        }
                    }

                    if (dz.Rows.Count > 0)
                    {
                        for (int i = 0; i < dz.Rows.Count; i++)
                        {
                            payments = new Payments();
                            payments.PaymentID = int.Parse(dz.Rows[i]["Payment ID"].ToString().Trim());
                            payments.AccountHolder = dz.Rows[i]["Account Holder"].ToString().Trim();
                            payments.BranchCode = dz.Rows[i]["Branch Code"].ToString().Trim();
                            payments.AccountNumber = dz.Rows[i]["Account Number"].ToString().Trim();
                            payments.AccountType = int.Parse(dz.Rows[i]["Account Type"].ToString().Trim());
                            payments.Amount = decimal.Parse(dz.Rows[i]["Amount"].ToString().Trim(), CultureInfo.InvariantCulture);
                            payments.EffectiveStatusDate = DateTime.ParseExact(dz.Rows[i]["Effective Status Date"].ToString().Trim(),"dd/MM/yyyy",CultureInfo.InvariantCulture);
                            payments.TransactionDate = DateTime.ParseExact(dz.Rows[i]["Transaction Date"].ToString().Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                            payments.Status = dz.Rows[i]["Status"].ToString().Trim();
                            //add to database
                            await _paymentRepository.AddPaymentAsync(payments);
                        }

                    }
                    TempData["mssg"] = "File Uploaded!";
                    return RedirectToAction("Index", "Home");

                }
                return PartialView("_UploadCSV");


            }
            catch (Exception ex)
            {
                TempData["mssg"] = ex.Message;
                return RedirectToAction("Index", "Home");
            }
          
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}