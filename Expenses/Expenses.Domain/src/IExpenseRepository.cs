using Expenses.Domain.Models;

namespace Expenses.Domain {
    public interface IExpenseRepository {
        IUnitOfWork UnitOfWork { get; }

        void AddExpenseCategory(ExpenseCategory category);
    }
}