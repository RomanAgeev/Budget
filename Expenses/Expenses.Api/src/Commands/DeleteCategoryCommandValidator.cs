using FluentValidation;

namespace Expenses.Api.Commands {
    public class DeleteCategoryCommandValidator : AbstractValidator<DeleteCategoryCommand> {
        public DeleteCategoryCommandValidator() {
            RuleFor(it => it.CategoryId)
                .GreaterThan(0);
        }
    }
}