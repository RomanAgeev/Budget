using System.Threading;
using System.Threading.Tasks;
using Expenses.Api.Utils;
using Expenses.Domain;
using Expenses.Domain.Models;
using Guards;
using MediatR;

namespace Expenses.Api.Commands {
    public class CreateExpenseCommandHandler : IRequestHandler<CreateExpenseCommand, int> {
        public CreateExpenseCommandHandler(IExpenseRepository repository) {
            Guard.NotNull(repository, nameof(repository));

            _repository = repository;
        }

        readonly IExpenseRepository _repository;

        public async Task<int> Handle(CreateExpenseCommand command, CancellationToken ct) {
            if(command.CategoryId == Constants.DefaultCategoryId)
                throw new DomainException(
                    cause: DomainExceptionCause.DefaultCategoryUpdateOrDelete,
                    message: "It is forbidden to add an expense to the default category"
                );

            Category category = await _repository.EnsureCategoryByIdAsync(command.CategoryId, ct);

            await _repository.LoadExpensesAsync(category, ct);

            Expense newExpense = category.AddExpense(command.Date, command.Amount, command.Description);

            await _repository.UnitOfWork.SaveAsync(ct);

            return newExpense.Id;
        }        
    }
}