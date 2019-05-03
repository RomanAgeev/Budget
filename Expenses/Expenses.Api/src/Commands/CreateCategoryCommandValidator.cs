using FluentValidation;

namespace Expenses.Api.Commands {
    public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand> {
        public CreateCategoryCommandValidator() {
            RuleFor(it => it.Name)
                .NotNull()
                .NotEmpty();
        }
    }
}