using System;
using System.ComponentModel.DataAnnotations;

namespace Assignment2.Models.ViewModel
{
    public class ListItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class InventoryProduct
    {
        public int Id { get; set; }

        public int IdProduct { get; set; }

        [Required(ErrorMessage = "Please inform the product name")]
        [MaxLength(128)]
        public string Name { get; set; }

        [Display(Name = "Quantity")]
        [Required(ErrorMessage = "Please, inform the quantity")]
        [Range(minimum: 1, maximum: 9999, ErrorMessage = "Please inform a quantity between 1 and 9999")]
        [RegularExpression("^[0-9]*$")]
        public int Qtd { get; set; }

        public int QtdAvailable { get; set; }

        [Required(ErrorMessage = "Please inform the price of acquisition for this product")]
        [Range(minimum: 0.01D, maximum: 10000D, ErrorMessage = "Please, inform a value between $0.01 and $10 000")]
        [DataType(DataType.Currency)]
        [Display(Name = "Unit Price (Acquisition)")]
        public decimal PriceAcquisition { get; set; }

        [Required(ErrorMessage = "Please inform the price for this product")]
        [Range(minimum: 0.01D, maximum: 10000D, ErrorMessage = "Please, inform a value between $0.01 and $10 000")]
        [DataType(DataType.Currency)]
        [Display(Name = "Unit Price (Sale)")]
        public decimal Price { get; set; }

        [Display(Name = "Date of Purchase")]
        [Required(ErrorMessage = "Please inform the date of purchase")]
        [DataType(DataType.Date)]
        [DateLEThanToday()]
        public DateTime DatePurchase { get; set; }

        [DataType(DataType.Date)]
        public string DatePurchaseStr
        {
            get
            {
                return this.DatePurchase.ToString("yyyy-MM-dd");
            }
            set
            {
                DatePurchase = DateTime.ParseExact(value, "yyyy-MM-dd", null);
            }
        }
    }

    /// <summary>
    /// Verifies if the date is lower or equal to today
    /// </summary>
    public class DateLEThanTodayAttribute : ValidationAttribute
    {
        public DateLEThanTodayAttribute(string errorMessage = "You must specify a date pior or equal to today")
        {
            this.ErrorMessage = errorMessage;
        }

        public override bool IsValid(object value)
        {
            bool result = true;

            if (value is DateTime)
            {
                if ((DateTime)value > DateTime.Now) { result = false; }
            }

            return result;
        }
    }
}