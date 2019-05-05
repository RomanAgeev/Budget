using FluentValidation;

namespace Expenses.Api.Commands {
    public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand> {
        public UpdateCategoryCommandValidator() {
            RuleFor(it => it.CategoryId)
                .GreaterThan(0);

            RuleFor(it => it.Name)
                .NotNull()
                .NotEmpty();
        }
    }
}