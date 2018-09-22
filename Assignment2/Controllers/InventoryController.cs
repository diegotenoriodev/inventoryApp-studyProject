using Assignment2.Models;
using Assignment2.Models.ViewModel;
using System.Web.Mvc;

namespace Assignment2.Controllers
{
    public class InventoryController : Controller
    {
        InventoryModel model = new InventoryModel();
        public ActionResult Index(string error = null, string success = null)
        {
            if (!string.IsNullOrEmpty(error))
            {
                ViewBag.MessageError = error;
            }
            else if (!string.IsNullOrEmpty(success))
            {
                ViewBag.MessageSuccess = success;
            }

            return View(model.GetAll());
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
            [Bind(Include = "Id,IdProduct,Name,Qtd,Price,PriceAcquisition,DatePurchase")]InventoryProduct product)
        {
            if (ModelState.IsValid)
            {
                ResultOperation result = model.Save(product);

                if (result.Success)
                {
                    return RedirectToAction("Index", new { success = "Item was successfully created" });
                }

                ModelState.AddModelError("Name", result.Message);
            }
            return View(product);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            InventoryProduct inventoryProduct = model.GetInventoryItem(id);

            if(inventoryProduct == null)
            {
                return RedirectToAction("Index", new { error = "Item was not found!" });
            }

            return View(inventoryProduct);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(
         [Bind(Include = "Id,IdProduct,Name,Qtd,Price,PriceAcquisition,DatePurchaseStr")]InventoryProduct product)
        {
            if (ModelState.IsValid)
            {
                ResultOperation result = model.Update(product);

                if (result.Success)
                {
                    return RedirectToAction("Index", new { success = "Item was successfully updated" });
                }

                ModelState.AddModelError("Name", result.Message);
            }

            return View(product);
        }

        public ActionResult Delete(int id)
        {
            ResultOperation result = model.Remove(id);

            if (result.Success)
            {
                return RedirectToAction("Index", new { success = "Item was successfully removed" });
            }
            else
            {
                return RedirectToAction("Index", new { error = result.Message });
            }
        }

        [HttpPost]
        public JsonResult GetProducts(string txt)
        {
            return Json(model.SearchProductsByName(txt));
        }

        [HttpPost]
        public JsonResult GetProductDetails(int id)
        {
            return Json(model.GetInventoryItemByProductId(id));
        }
    }
}