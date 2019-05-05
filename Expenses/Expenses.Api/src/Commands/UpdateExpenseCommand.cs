namespace Expenses.Api.Commands {
    public class UpdateExpenseCommand : CommandBase {
        public int ExpenseId { get; set; }
        public int CategoryId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
    }
}