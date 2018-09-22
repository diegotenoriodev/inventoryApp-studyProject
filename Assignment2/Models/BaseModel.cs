using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Assignment2.Models
{
    public class ResultOperation
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }

    public class BaseModel
    {
        private ProductsDBEntities db;
        public ProductsDBEntities DB
        {
            get
            {
                if (db == null)
                {
                    db = new ProductsDBEntities();
                }

                return db;
            }
        }

        protected ResultOperation SurroundWithTryCatch(Func<string> func)
        {
            string error = string.Empty;

            try
            {
                error = func();
            }
            catch (Exception ex)
            {
                AddLog(ex);
                error = "An unexpected error occurred. If the problem persists, please contact the system admin";
            }

            return new ResultOperation() { Success = string.IsNullOrEmpty(error), Message = error };
        }

        private void AddLog(Exception ex)
        {
            //TODO: Add to log
        }
    }

}