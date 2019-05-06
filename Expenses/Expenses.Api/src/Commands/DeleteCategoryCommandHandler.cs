using System.Threading;
using System.Threading.Tasks;
using Expenses.Api.Utils;
using Expenses.Domain;
using Expenses.Domain.Models;

namespace Expenses.Api.Commands {
    public class DeleteCategoryCommandHandler : CommandHandlerBase<DeleteCategoryCommand> {
        public DeleteCategoryCommandHandler(IExpenseRepository repository)
            : base(repository) {
        }

        public override async Task<bool> Handle(DeleteCategoryCommand command, CancellationToken ct) {
            Category defaultCategory = await Repository.GetCategoryByIdAsync(Constants.DefaultCategoryId, ct);

            Category category = await Repository.EnsureCategoryByIdAsync(command.CategoryId, ct);

            await Repository.LoadExpenses(defaultCategory, ct);
            await Repository.LoadExpenses(category, ct);

            category.MoveExpenses(defaultCategory);

            Repository.DeleteCategory(category);

            await Repository.UnitOfWork.SaveAsync(ct);


            return true;
        }
    }
}