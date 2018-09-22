namespace Assignment2.Models.ViewModel
{
    public class ProductReport
    {
        public string Name { get; set; }
        public int Qty { get; set; }
    }

    public class AverageProfitProduct
    {
        public string Name { get; set; }
        public decimal AverageSalePrice { get; set; }
        public decimal AveragePurchasePrice { get; set; }
        public int QttSold { get; set; }

        public decimal AverageProfit
        {
            get
            {
                return (AverageSalePrice - AveragePurchasePrice) * QttSold;
            }
        }
    }
}