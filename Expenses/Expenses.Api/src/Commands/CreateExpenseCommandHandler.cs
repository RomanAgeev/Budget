using System.Threading;
using System.Threading.Tasks;
using Expenses.Domain;
using Expenses.Domain.Models;

namespace Expenses.Api.Commands {
    public class CreateExpenseCommandHandler : CommandHandlerBase<CreateExpenseCommand> {
        public CreateExpenseCommandHandler(IExpenseRepository repository)
            : base(repository) {
        }

        public override async Task<bool> Handle(CreateExpenseCommand command, CancellationToken cancellationToken) {
             Category category = await Repository.GetCategoryAsync(command.CategoryId, cancellationToken);
             if(category == null)
                throw new DomainException(DomainExceptionCause.CategoryNotFound, $"Category with {command.CategoryId} ID is not found"); 

            category.AddExpense(command.Date, command.Amount, command.Description);

            await Repository.UnitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }        
    }
}