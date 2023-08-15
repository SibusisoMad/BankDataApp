using System.ComponentModel.DataAnnotations;

namespace BankDataApp.Models
{
    public class Payments
    {
        [Key]
        public int Id { get; set; }
        public int PaymentID { get; set; }
        public string AccountHolder { get; set; }
        public string BranchCode { get; set; }
        public string AccountNumber { get; set; }
        public int AccountType { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public DateTime EffectiveStatusDate { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}
