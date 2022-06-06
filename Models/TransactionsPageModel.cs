using System;
using System.Collections.Generic;
using System.Text;

namespace MoneySaver.SPA.Models
{
    public class TransactionsPageModel
    {
        public IEnumerable<Transaction> Result { get; set; }

        public int TotalCount { get; set; }
    }
}
