namespace Expenses.Api.Commands {
    public class DeleteExpenseCommand : CommandBase {
        public int ExpenseId { get; set; }
    }
}