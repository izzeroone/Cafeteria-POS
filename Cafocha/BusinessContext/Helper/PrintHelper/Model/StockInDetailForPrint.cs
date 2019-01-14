namespace Cafocha.BusinessContext.Helper.PrintHelper.Model
{
    public class StockInDetailForPrint
    {
        public string Name { get; set; } // product_id (Primary key) (length: 10)
        public double Quan { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice { get; set; }
        public string Note { get; set; }
    }
}
