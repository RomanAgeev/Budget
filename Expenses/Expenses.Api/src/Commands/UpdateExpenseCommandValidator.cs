using FluentValidation;

namespace Expenses.Api.Commands {
    public class UpdateExpenseCommandValidator : AbstractValidator<UpdateExpenseCommand> {
        public UpdateExpenseCommandValidator() {
            RuleFor(it => it.ExpenseId)
                .GreaterThan(0);

            RuleFor(it => it.Amount)
                .GreaterThan(0);
        }
    }
}