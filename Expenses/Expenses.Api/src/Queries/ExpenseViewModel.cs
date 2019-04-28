using System;

namespace Expenses.Api.Queries {
    public class ExpenseViewModel {
        public string Description { get; set; }
        public string Category { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
    }
}