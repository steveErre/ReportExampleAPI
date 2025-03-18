namespace ReportExampleAPI.Models
{
    public class StockPrice
    {
        public string Symbol { get; set; }
        public decimal CurrentPrice { get; set; }
        public decimal PercentageChange { get; set; }
    }
}
