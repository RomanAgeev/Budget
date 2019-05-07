using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Expenses.Domain.Models;

namespace Expenses.Domain {
    public interface IExpenseRepository {
        IUnitOfWork UnitOfWork { get; }

        Task<Category> GetCategoryByIdAsync(int categoryId, CancellationToken ct);
        Task<Category> GetCategoryByNameAsync(string categoryName, CancellationToken ct);
        Task LoadExpensesAsync(Category category, CancellationToken ct);
        void AddCategory(Category category);
        void RemoveCategory(Category category);
        Task<Expense> GetExpenseByIdAsync(int expenseId, CancellationToken ct);
        void RemoveExpense(Expense expense);
        int GetExpenseCategoryId(Expense expense);
    }
}