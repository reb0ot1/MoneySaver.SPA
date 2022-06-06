using System;
using System.Collections.Generic;
using System.Text;

namespace MoneySaver.SPA.Models
{
    public class TransactionCategory
    {
        public int? TransactionCategoryId { get; set; }
        public string Name { get; set; }
        public int? ParentId { get; set; }
        public string AlternativeName { get; set; }
        public List<TransactionCategory> Children { get; set; } = new List<TransactionCategory>();
    }
}
