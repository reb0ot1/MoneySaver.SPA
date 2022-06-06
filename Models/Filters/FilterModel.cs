using System;
using System.Collections.Generic;
using System.Text;

namespace MoneySaver.SPA.Models.Filters
{
    public class FilterModel
    {
        public DateTime From { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

        public DateTime To { get; set; } = DateTime.Now;
    }
}
