using System.Collections.Generic;

namespace Expenses.Domain.Models {
    public class ExpenseCategory {
        public int ExpenseCategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; } 
        public List<Expense> Expenses { get; set; }
    }    
}