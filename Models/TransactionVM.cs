namespace MoneySaver.SPA.Models
{
    public class TransactionVM
    {
        public int Index;

        public DateTime TransactionDate { get; set; }

        public int TransactionCategoryId { get; set; }

        public double Amount { get; set; }

        public string AdditionalNote { get; set; }
    }
}
