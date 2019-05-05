using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Expenses.Domain;
using Expenses.Domain.Models;
using Guards;
using Microsoft.EntityFrameworkCore;

namespace Expenses.Infrastructure {
    public class ExpenseRepository : IExpenseRepository {
        public ExpenseRepository(ExpenseContext context) {
            Guard.NotNull(context, nameof(context));

            _context = context;
        }

        readonly ExpenseContext _context;

        public IUnitOfWork UnitOfWork => _context;

        public IEnumerable<Category> GetCategories() {
            return _context.Categories;
        }
        public Task<Category> GetCategoryAsync(int categoryId, CancellationToken cancellationToken = default(CancellationToken)) {
            return _context.Categories
                .Include(it => it.Expenses)
                .Where(it => it.Id == categoryId)
                .SingleOrDefaultAsync();
        }
        public void AddCategory(Category category) {
            Guard.NotNull(category, nameof(category));

            _context.Categories.Add(category);
        }
        public void DeleteCategory(Category category) {
            Guard.NotNull(category, nameof(category));

            _context.Categories.Remove(category);
        }

        public Task<Expense> GetExpenseAsync(int expenseId, CancellationToken cancellationToken = default(CancellationToken)) {
            return _context.Expenses
                .Where(it => it.Id == expenseId)
                .SingleOrDefaultAsync(cancellationToken);
        }

        public void DeleteExpense(Expense expense) {
            Guard.NotNull(expense, nameof(expense));

            _context.Expenses.Remove(expense);
        }

        public Task<Category> GetContainingCategoryAsync(Expense expense, CancellationToken cancellationToken = default(CancellationToken)) {
            Guard.NotNull(expense, nameof(expense));

            int categoryId = _context.Entry(expense).Property<int>("CategoryId").CurrentValue;
            return GetCategoryAsync(categoryId, cancellationToken);
        }
    }
}