using System.Collections.Generic;
using System.Web.Mvc;

namespace HouseBills.Models
{
    public class DebtModel
    {
        public DebtModel()
        {
            People = new List<SelectListItem>();
        }

        public decimal Amount { get; set; }
        public string Description { get; set; }
        public List<SelectListItem> People { get; set; }
        public int PersonId { get; set; }
    }
}