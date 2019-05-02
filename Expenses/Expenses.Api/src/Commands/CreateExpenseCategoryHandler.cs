using System.Threading;
using System.Threading.Tasks;
using Expenses.Domain;
using Expenses.Domain.Models;
using Guards;
using MediatR;

namespace Expenses.Api.Commands {
    public class CreateExpenseCategoryHandler : IRequestHandler<CreateExpenseCategoryCommand, bool> {
        public CreateExpenseCategoryHandler(IExpenseRepository repository) {
            Guard.NotNull(repository, nameof(repository));

            _repository = repository;
        }

        readonly IExpenseRepository _repository;

        public async Task<bool> Handle(CreateExpenseCategoryCommand command, CancellationToken cancellationToken) {
            var category = new ExpenseCategory(command.Name, command.Description);

            _repository.AddExpenseCategory(category);

            int count = await _repository.UnitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}