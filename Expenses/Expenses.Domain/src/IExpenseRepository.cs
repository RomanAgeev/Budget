using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Expenses.Domain.Models;

namespace Expenses.Domain {
    public interface IExpenseRepository {
        IUnitOfWork UnitOfWork { get; }

        IEnumerable<Category> GetCategories();
        Category GetCategory(int categoryId);
        void AddCategory(Category category);
        void DeleteCategory(Category category);
        Task<Expense> GetExpenseAsync(int expenseId, CancellationToken cancellationToken);
        void DeleteExpense(Expense expense);
    }
}