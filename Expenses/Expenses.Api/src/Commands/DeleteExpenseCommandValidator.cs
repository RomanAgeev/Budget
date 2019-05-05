using FluentValidation;

namespace Expenses.Api.Commands {
    public class DeleteExpenseCommandValidator : AbstractValidator<DeleteExpenseCommand> {
        public DeleteExpenseCommandValidator() {
            RuleFor(it => it.ExpenseId)
                .GreaterThan(0);
        }
    }
}