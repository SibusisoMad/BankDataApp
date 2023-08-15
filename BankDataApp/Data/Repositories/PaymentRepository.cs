using AutoMapper;
using BankDataApp.Interfaces;
using BankDataApp.Models;
using Microsoft.EntityFrameworkCore;
using NuGet.Configuration;
using System.Globalization;

namespace BankDataApp.Data.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public PaymentRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;

        }

        public async Task<bool> AddPaymentAsync(Payments payments)
        {
            _context.Entry(payments).State = EntityState.Added;
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<Master>> GetMasterDetails()
        {
            var payments = await _context.Payments.ToListAsync();
            List<Master> details =
                payments.Select(x => 
                {
                    string accountType = "";
                    if(x.AccountType == 1)
                    {
                        accountType = "Current / Cheque";
                    }
                    if (x.AccountType == 2)
                    {
                        accountType = "Savings";
                    }
                    return new Master
                    {
                        AccountHolder = x.AccountHolder,
                        AccountNumber = x.AccountNumber,
                        BranchCode = x.BranchCode,
                        AccountType = accountType
                    };
                }).ToList();
            List<Master> distinctmasterDetails = details.GroupBy(x => x.AccountHolder).Select(g => g.First()).ToList();
            return distinctmasterDetails;

        }

        public async Task<IEnumerable<Report>> GetReport()
        {
            var payments = await _context.Payments.ToListAsync();

            var results = (from p in _context.Payments
                          group p by new { p.BranchCode, p.AccountType, p.Status } into g
                          
                          select new Report
                          {
                              BranchCode = g.Key.BranchCode,
                              AccountType = g.Key.AccountType.ToString(),
                              Status = g.Key.Status,
                              TotalCount = g.Count(p => p.PaymentID != 0).ToString(),
                              TotalAmount = g.Sum(p => p.Amount)
                          }).ToList();

            List<Report> details =
                results.Select(x =>
                {
                    string accountType = x.AccountType == "1" ? "Current / Cheque" : "Savings";
                    string status = x.Status == "00" ? "Successful" : x.Status == "30" ? "Disputed" : "Failed";
                    return new Report
                    {
                        Status = status,
                        TotalAmount = x.TotalAmount,
                        TotalCount = x.TotalCount,
                        BranchCode = x.BranchCode,
                        AccountType = accountType
                    };
                }).ToList();


            return details;
        }


        public async Task<IEnumerable<Detail>> GetDetails(string accNumber)
        {
            var payments = await _context.Payments.Where(x => x.AccountNumber == accNumber).ToListAsync();
            List<Detail> details =
                payments.Select(x =>
                {
                    string status = "";
                    if (x.Status =="00")
                    {
                        status = "Successful";
                    }
                    else if (x.Status == "30")
                    {
                        status = "Disputed";
                    }
                    else
                    {
                        status = "Failed";
                    }
                    TimeSpan difference = x.EffectiveStatusDate - x.TransactionDate;
                    bool isBreached = difference.Days > 7;
                    string timeBreached = isBreached ? "Yes" : "No";

                    return new Detail
                    {
                        TransactionDate  = x.TransactionDate.ToString(),
                        Status = status,
                        Amount = x.Amount,
                        EffectiveStatusDate = x.EffectiveStatusDate.ToString(),
                        TimeBreached = timeBreached
                    };
                }).ToList();


            return details;

        }


    }
}
