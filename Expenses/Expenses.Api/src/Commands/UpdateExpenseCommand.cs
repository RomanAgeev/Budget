using System.Threading;
using System.Threading.Tasks;
using Expenses.Api.Utils;
using Expenses.Domain;
using Expenses.Domain.Models;
using FluentValidation;
using Guards;
using MediatR;

namespace Expenses.Api.Commands {
    public class UpdateExpenseCommand : IRequest<bool> {
        public class Validator : AbstractValidator<UpdateExpenseCommand> {
            public Validator() {
                RuleFor(it => it.ExpenseId)
                    .GreaterThan(0);
                
                RuleFor(it => it.CategoryId)
                    .GreaterThan(0);

                RuleFor(it => it.Amount)
                    .GreaterThan(0);
            }
        }

        public int ExpenseId { get; set; }
        public int CategoryId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
    }

     public class UpdateExpenseCommandHandler : IRequestHandler<UpdateExpenseCommand, bool> {
        public UpdateExpenseCommandHandler(IExpenseRepository repository) {
            Guard.NotNull(repository, nameof(repository));

            _repository = repository;
        }

        readonly IExpenseRepository _repository;

        public async Task<bool> Handle(UpdateExpenseCommand command, CancellationToken ct) {
            if(command.CategoryId == Constants.DefaultCategoryId)
                throw new DomainException(
                    cause: DomainExceptionCause.DefaultCategoryUpdateOrDelete,
                    message: "It is forbidden to move an expense to the default category"
                );

            Expense expense = await _repository.EnsureExpenseByIdAsync(command.ExpenseId, ct);

            int expenseCategoryId = _repository.GetExpenseCategoryId(expense);
            
            Category fromCategory = await _repository.EnsureCategoryByIdAsync(expenseCategoryId, ct);
            Category toCategory = await _repository.EnsureCategoryByIdAsync(command.CategoryId, ct);

            if(toCategory != fromCategory) {
                await _repository.LoadExpensesAsync(fromCategory, ct);
                await _repository.LoadExpensesAsync(toCategory, ct);

                fromCategory.MoveExpense(toCategory, expense);
            }

            expense.Update(command.Amount, command.Description);
 
            await _repository.UnitOfWork.SaveAsync(ct);

            return true;
        }
    }
}