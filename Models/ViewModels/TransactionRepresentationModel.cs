namespace MoneySaver.SPA.Models.ViewModels
{
    public struct TransactionRepresentationModel
    {
        public string Id { get; set; }

        public DateTime DateTime { get; set; }

        public string CategoryName { get; set; }

        public double? Amount { get; set; }

        public string Notes { get; set; }
    }
}
