using System;
using System.Linq;
using Assignment2.Models.ViewModel;

namespace Assignment2.Models
{
    public class InventoryModel : BaseModel
    {
        public ResultOperation Save(InventoryProduct invetoryProduct)
        {
            return SurroundWithTryCatch(() =>
            {
                Product product = CreateProductIfNotExists(invetoryProduct.IdProduct, invetoryProduct.Name);

                product.ProductEntries.Add(new ProductEntry()
                {
                    PurchaseDate = invetoryProduct.DatePurchase,
                    QuantityBought = invetoryProduct.Qtd,
                    QuantityAvailable = invetoryProduct.Qtd,
                    UnitPrice = invetoryProduct.Price,
                    UnitPriceAcquisition = invetoryProduct.PriceAcquisition
                });

                DB.SaveChanges();

                return string.Empty;
            });
        }

        public InventoryProduct GetInventoryItem(int id)
        {
            var query = from p in DB.Products
                        join e in DB.ProductEntries
                            on p.Id equals e.IdProduct
                        where e.Id == id
                        select new InventoryProduct()
                        {
                            DatePurchase = e.PurchaseDate,
                            Id = e.Id,
                            IdProduct = p.Id,
                            Name = p.Name,
                            Price = e.UnitPrice,
                            PriceAcquisition = e.UnitPriceAcquisition,
                            Qtd = e.QuantityBought
                        };

            return query.SingleOrDefault();
        }

        public InventoryProduct[] GetAll()
        {
            var query = from p in DB.Products
                        join e in DB.ProductEntries
                            on p.Id equals e.IdProduct
                        orderby p.Name, e.PurchaseDate descending
                        select new InventoryProduct()
                        {
                            DatePurchase = e.PurchaseDate,
                            Id = e.Id,
                            IdProduct = p.Id,
                            Name = p.Name,
                            Price = e.UnitPrice,
                            Qtd = e.QuantityBought,
                            PriceAcquisition = e.UnitPriceAcquisition,
                            QtdAvailable = e.QuantityAvailable
                        };

            return query.ToArray();
        }

        public ResultOperation Remove(int id)
        {
            return SurroundWithTryCatch(() =>
                {
                    string error = string.Empty;

                    ProductEntry entry = DB.ProductEntries.SingleOrDefault(r => r.Id == id);

                    if (entry == null)
                    {
                        error = "Item was not found.";
                    }
                    else
                    {
                        if (entry.SaleItems.Any())
                        {
                            error = "You cannot remove this product! There are sales related to this item.";
                        }
                        else
                        {
                            DB.ProductEntries.Remove(entry);
                            DB.SaveChanges();
                        }
                    }

                    return error;
                }
            );
        }

        public InventoryProduct GetInventoryItemByProductId(int id)
        {
            InventoryProduct item = (from e in DB.ProductEntries
                                     join p in DB.Products on e.IdProduct equals p.Id
                                     where p.Id == id
                                     orderby e.PurchaseDate descending
                                     select new InventoryProduct()
                                     {
                                         IdProduct = p.Id,
                                         Name = p.Name,
                                         Qtd = e.QuantityBought,
                                         Price = e.UnitPrice,
                                         PriceAcquisition = e.UnitPriceAcquisition,
                                         DatePurchase = DateTime.Now
                                     }).FirstOrDefault();

            return item;
        }

        public ListItem[] SearchProductsByName(string txt)
        {
            var query = from r in DB.Products
                        where r.Name.Contains(txt)
                        orderby r.Name
                        select new ListItem() { Id = r.Id, Name = r.Name };

            return query.ToArray();
        }

        public ResultOperation Update(InventoryProduct inventoryProduct)
        {
            return SurroundWithTryCatch(() =>
            {
                string error = string.Empty;

                ProductEntry entry = DB.ProductEntries.Include("SaleItems")
                                        .FirstOrDefault(r => r.Id == inventoryProduct.Id);

                if (entry == null)
                {
                    error = "Inventory Item was not found.";
                }
                else if(entry.QuantityAvailable == 0 && entry.QuantityBought > inventoryProduct.Qtd)
                {
                    error = $"This product has sold {entry.QuantityBought } unities. You must specify in quantity bought a number higher than it";
                }
                else
                {
                    entry.PurchaseDate = inventoryProduct.DatePurchase;
                    entry.UnitPrice = inventoryProduct.Price;
                    entry.QuantityAvailable += (inventoryProduct.Qtd - entry.QuantityBought);
                    entry.QuantityBought = inventoryProduct.Qtd;
                    entry.UnitPriceAcquisition = inventoryProduct.PriceAcquisition;

                    DB.SaveChanges();
                }

                return error;
            });
        }

        private Product CreateProductIfNotExists(int idProduct, string name)
        {
            Product product = null;

            if (idProduct != 0)
            {
                product = DB.Products.FirstOrDefault(r => r.Id == idProduct);
            }
            else
            {
                product = new Product()
                {
                    Name = name
                };

                DB.Products.Add(product);
            }

            return product;
        }
    }
}