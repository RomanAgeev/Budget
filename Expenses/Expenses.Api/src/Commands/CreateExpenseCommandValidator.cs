using FluentValidation;

namespace Expenses.Api.Commands {
    public class CreateExpenseCommandValidator : AbstractValidator<CreateExpenseCommand> {
        public CreateExpenseCommandValidator() {
            RuleFor(it => it.Amount)
                .GreaterThan(0);
            RuleFor(it => it.CategoryId)
                .GreaterThan(0);
        }        
    }

}