using Assignment2.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assignment2.Models
{
    public class SaleModel : BaseModel
    {
        private string errorMessage = string.Empty;

        public InventoryProduct GetInventoryItemByProductId(int id)
        {
            InventoryProduct item = (from e in DB.ProductEntries
                                     join p in DB.Products on e.IdProduct equals p.Id
                                     where e.Id == id
                                     select new InventoryProduct()
                                     {
                                         Id = e.Id,
                                         Name = p.Name,
                                         Price = e.UnitPrice,
                                         QtdAvailable = p.ProductEntries.Where(r => r.UnitPrice == e.UnitPrice).Sum(r => r.QuantityAvailable)
                                     }).FirstOrDefault();

            return item;
        }

        public ListItem[] SearchProductsByName(string txt)
        {
            var query = from r in DB.Products
                        join e in DB.ProductEntries on r.Id equals e.IdProduct
                        where r.Name.Contains(txt)
                        && e.QuantityAvailable > 0
                        orderby r.Name
                        select new { e.Id, e.IdProduct, r.Name, e.UnitPrice };

            List<ListItem> items = new List<ListItem>();

            string name = string.Empty;

            // This is to avoid showing two items from the same product with the same price
            foreach (var item in query)
            {
                name = item.Name + " (" + item.UnitPrice.ToString("c") + ")";

                if (!items.Any(r => r.Name == name))
                {
                    items.Add(new ListItem() { Id = item.Id, Name = name });
                }
            }

            return items.ToArray();
        }

        public ResultOperation SavePurchase(DateTime date, PurchaseItem[] items)
        {
            if (items.Any(r => r.Qtd <= 0))
            {
                return new ResultOperation() { Success = false, Message = "There are invalid quantities in this request. Please review before saving purchase!" };
            }

            using (var transaction = DB.Database.BeginTransaction(System.Data.IsolationLevel.RepeatableRead))
            {
                try
                {
                    // Creating the header of the sale
                    Sale sale = new Sale() { Date = date };

                    // Items that will be worked through loop
                    SaleItem saleItem = null;
                    ProductEntry entry = null;

                    // For each item that came from the client
                    foreach (var item in items)
                    {
                        //Looking for the specific entry and not the product itself that is in a different table
                        entry = DB.ProductEntries.FirstOrDefault(r => r.Id == item.IdProduct);

                        //Something must be wrong, the entry was not found
                        if (entry == null)
                        {
                            errorMessage += $"Product {item.productName} was not found!<br/>";
                            break;
                        }
                        else
                        {
                            // Something happened and the quantity requested is not available
                            if (entry.QuantityAvailable < item.Qtd)
                            {
                                VerifyOtherEntries(entry, item);
                            }
                            else
                            {
                                // Here is where the transaction is useful
                                // if someone else is changing the QuantityAvailable, 
                                // we need to avoid that this transaction override the update in a concurrent environment
                                entry.QuantityAvailable -= item.Qtd;
                            }

                            if (string.IsNullOrEmpty(errorMessage))
                            {
                                // Creating one item for the sale
                                saleItem = new SaleItem()
                                {
                                    ProductEntry = entry,
                                    UnitPrice = entry.UnitPrice,
                                    Quantity = item.Qtd
                                };

                                // You can do this, or just set the property Sale from saleItem with 'sale'
                                sale.SaleItems.Add(saleItem);
                            }
                        }
                    }

                    DB.Sales.Add(sale);
                }
                catch (Exception ex)
                {
                    errorMessage = ex.Message;
                }

                if (string.IsNullOrEmpty(errorMessage))
                {
                    try
                    {
                        // You must call save changes first
                        DB.SaveChanges();
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        errorMessage = "The server is busy! Try again, if the problem persists, get in touch with the System Admin";
                    }
                }
                else
                {
                    transaction.Rollback();
                }
            }

            return new ResultOperation() { Success = errorMessage == string.Empty, Message = errorMessage };
        }

        private void VerifyOtherEntries(ProductEntry entry, PurchaseItem item)
        {
            List<ProductEntry> entries = GetProductEntriesWithSamePrice(entry.IdProduct, entry.UnitPrice);
            int qttAvail = entries.Sum(r => r.QuantityAvailable);

            if (qttAvail >= item.Qtd)
            {
                int qttAux = item.Qtd;
                foreach (var ent in entries)
                {
                    if (qttAux == 0) { break; }

                    if (ent.QuantityAvailable <= qttAux)
                    {
                        qttAux -= ent.QuantityAvailable;
                        ent.QuantityAvailable = 0;
                    }
                    else
                    {
                        ent.QuantityAvailable -= qttAux;
                        qttAux = 0;
                    }
                }
            }
            else
            {
                errorMessage = $"Product {item.productName} has only {qttAvail} available unities.<br/>";
            }
        }

        private List<ProductEntry> GetProductEntriesWithSamePrice(int idProduct, decimal unitPrice)
        {
            return DB.ProductEntries.Where(
                    r => r.IdProduct == idProduct && r.UnitPrice == unitPrice
                ).ToList();
        }
    }
}