using Expenses.Domain.Models;

namespace Expenses.Domain {
    public interface IExpenseRepository {
        IUnitOfWork UnitOfWork { get; }

        void AddCategory(Category category);
    }
}