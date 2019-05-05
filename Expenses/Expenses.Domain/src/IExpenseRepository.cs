using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Expenses.Domain.Models;

namespace Expenses.Domain {
    public interface IExpenseRepository {
        IUnitOfWork UnitOfWork { get; }

        IEnumerable<Category> GetCategories();
        Task<Category> GetCategoryAsync(int categoryId, CancellationToken cancellationToken);
        void AddCategory(Category category);
        void DeleteCategory(Category category);
        Task<Expense> GetExpenseAsync(int expenseId, CancellationToken cancellationToken);
        void DeleteExpense(Expense expense);
        Task<Category> GetContainingCategoryAsync(Expense expense, CancellationToken cancellationToken);
    }
}