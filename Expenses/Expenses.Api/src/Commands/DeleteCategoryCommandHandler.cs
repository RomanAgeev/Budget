using System.Threading;
using System.Threading.Tasks;
using Expenses.Api.Utils;
using Expenses.Domain;
using Expenses.Domain.Models;
using Guards;
using MediatR;

namespace Expenses.Api.Commands {
    public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, bool> {
        public DeleteCategoryCommandHandler(IExpenseRepository repository) {
            Guard.NotNull(repository, nameof(repository));

            _repository = repository;
        }

        readonly IExpenseRepository _repository;

        public async Task<bool> Handle(DeleteCategoryCommand command, CancellationToken ct) {
            if(command.CategoryId == Constants.DefaultCategoryId)
                throw new DomainException(
                    cause: DomainExceptionCause.DefaultCategoryUpdateOrDelete,
                    message: "It is forbidden to delete the default category"
                );

            Category category = await _repository.EnsureCategoryByIdAsync(command.CategoryId, ct);
            Category defaultCategory = await _repository.GetCategoryByIdAsync(Constants.DefaultCategoryId, ct);

            await _repository.LoadExpensesAsync(category, ct);
            await _repository.LoadExpensesAsync(defaultCategory, ct);

            category.MoveExpenses(defaultCategory);

            _repository.RemoveCategory(category);

            await _repository.UnitOfWork.SaveAsync(ct);

            return true;
        }
    }
}