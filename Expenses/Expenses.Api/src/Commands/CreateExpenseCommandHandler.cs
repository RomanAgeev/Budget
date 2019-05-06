using System.Threading;
using System.Threading.Tasks;
using Expenses.Domain;
using Expenses.Domain.Models;

namespace Expenses.Api.Commands {
    public class CreateExpenseCommandHandler : CommandHandlerBase<CreateExpenseCommand> {
        public CreateExpenseCommandHandler(IExpenseRepository repository)
            : base(repository) {
        }

        public override async Task<bool> Handle(CreateExpenseCommand command, CancellationToken ct) {
             Category category = await Repository.GetCategoryByIdAsync(command.CategoryId, ct);
             if(category == null)
                throw new DomainException(DomainExceptionCause.CategoryNotFound, $"Category with {command.CategoryId} ID is not found"); 

            await Repository.LoadExpenses(category, ct);

            category.AddExpense(command.Date, command.Amount, command.Description);

            await Repository.UnitOfWork.SaveAsync(ct);

            return true;
        }        
    }
}