using Assignment2.Models.ViewModel;
using System;
using System.Linq;

namespace Assignment2.Models
{
    public class Period
    {
        public DateTime? InitialDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class ReportsModel : BaseModel
    {
        public ProductReport[] GetProductsSold(Period period = null)
        {
            var query = from s in DB.Sales
                        join i in DB.SaleItems on s.Id equals i.IdSale
                        join e in DB.ProductEntries on i.IdProduct equals e.Id
                        join p in DB.Products on e.IdProduct equals p.Id
                        select new { p.Name, i.Quantity, s.Date };

            if (period != null)
            {
                if (period.EndDate.HasValue)
                {
                    query = query.Where(r => r.Date <= period.EndDate.Value);
                }

                if (period.InitialDate.HasValue)
                {
                    query = query.Where(r => r.Date >= period.InitialDate);
                }
            }

            return (from r in query
                    group r.Quantity by r.Name into g
                    select new ProductReport() { Name = g.Key, Qty = g.Sum() }).ToArray();
        }

        public ProductReport[] GetProductsPurchased(Period period = null)
        {
            var query = from p in DB.Products
                        join e in DB.ProductEntries on p.Id equals e.IdProduct
                        select new { p.Name, Quantity = e.QuantityBought, Date = e.PurchaseDate };

            if (period != null)
            {
                if (period.EndDate.HasValue)
                {
                    query = query.Where(r => r.Date <= period.EndDate.Value);
                }

                if (period.InitialDate.HasValue)
                {
                    query = query.Where(r => r.Date >= period.InitialDate);
                }
            }

            return (from r in query
                    group r.Quantity by r.Name into g
                    select new ProductReport() { Name = g.Key, Qty = g.Sum() }).ToArray();
        }

        public ProductReport[] GetAvailableQuantity()
        {
            var query = from p in DB.Products
                        join e in DB.ProductEntries on p.Id equals e.IdProduct
                        where e.QuantityAvailable > 0
                        select new
                        {
                            Name = p.Name + " (seling at " + e.UnitPrice.ToString()
                        + ")",
                            Quantity = e.QuantityAvailable
                        };

            return (from r in query
                    group r.Quantity by r.Name into g
                    select new ProductReport() { Name = g.Key, Qty = g.Sum() }).ToArray();

        }

        public AverageProfitProduct[] GetAverageProfitReport()
        {
            var query = from p in DB.Products
                        join e in DB.ProductEntries on p.Id equals e.IdProduct
                        group new
                        {
                            e.UnitPriceAcquisition,
                            e.UnitPrice,
                            ItemsSold = e.QuantityBought - e.QuantityAvailable
                        }
                        by new { p.Id, p.Name } into m
                        select new AverageProfitProduct()
                        {
                            Name = m.Key.Name,
                            AveragePurchasePrice = m.Average(r => r.UnitPriceAcquisition),
                            AverageSalePrice = m.Average(r => r.UnitPrice),
                            QttSold = m.Sum(r => r.ItemsSold),
                        };

            return query.OrderBy(r => r.Name).ToArray();
        }
    }
}