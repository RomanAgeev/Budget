namespace Expenses.Domain.Models {
    public class Expense {
        public int ExpenseId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        
        public int CategoryId { get; set; }
        public ExpenseCategory Category { get; set; }
    }
}