namespace Cafocha.BusinessContext.Helper.PrintHelper.Model
{
    public class ReceiptEntityForReport
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int BillCount { get; set; }
        public double? InStock { get; set; }
        public decimal TotalAmount { get; set; }
    }
}