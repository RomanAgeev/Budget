using MediatR;

namespace Expenses.Api.Commands {
    public class DeleteExpenseCommand : IRequest<bool> {
        public int ExpenseId { get; set; }
    }
}