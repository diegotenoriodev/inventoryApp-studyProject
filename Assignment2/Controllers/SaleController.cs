using Assignment2.Models;
using Assignment2.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Assignment2.Controllers
{
    public class SaleController : Controller
    {
        SaleModel model = new SaleModel();
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetInventoryProducts(string txt)
        {
            return Json(model.SearchProductsByName(txt));
        }

        [HttpPost]
        public JsonResult GetInventoryProductDetails(int id)
        {
            return Json(model.GetInventoryItemByProductId(id));
        }

        [HttpPost]
        public JsonResult FinishPurchase(Purchase purchase)
        {
             return Json(model.SavePurchase(purchase.date, purchase.items));
        }

        public class Purchase
        {
            public DateTime date { get; set; }
            public PurchaseItem[] items { get; set; }
        }
    }
}