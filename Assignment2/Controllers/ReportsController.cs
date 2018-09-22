using Assignment2.Models;
using System;
using System.Web.Mvc;

namespace Assignment2.Controllers
{
    public class ReportsController : Controller
    {
        ReportsModel model = new ReportsModel();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ProductsSold()
        {
            ViewBag.Action = "ProductSoldList";
            return PartialView(viewName: "ProductsPerRange");
        }

        public ActionResult ProductSoldList(DateTime? initialDate, DateTime? endDate)
        {
            ViewBag.Action = "ProductSoldList";
            ViewBag.ReportName = "Products Sold Per Date Range";
            return PartialView("ProductReportList", model.GetProductsSold(new Period { InitialDate = initialDate, EndDate = endDate }));
        }

        public ActionResult ProductsPurchased()
        {
            ViewBag.Action = "ProductsPurchasedList";
            return PartialView(viewName: "ProductsPerRange");
        }

        public ActionResult ProductsPurchasedList(DateTime? initialDate, DateTime? endDate)
        {
            ViewBag.Action = "ProductsPurchasedList";
            ViewBag.ReportName = "Products Purchased Per Date Range";
            return PartialView("ProductReportList", model.GetProductsPurchased(new Period { InitialDate = initialDate, EndDate = endDate }));
        }

        public ActionResult AvailableQuantity()
        {
            ViewBag.ReportName = "Products - Quantity Available";
            return PartialView("ProductReportList", model.GetAvailableQuantity());
        }

        public ActionResult AverageProfit()
        {
            ViewBag.ReportName = "Products - Average Profit";
            return PartialView("AverageProfitProductList", model.GetAverageProfitReport());
        }
    }
}