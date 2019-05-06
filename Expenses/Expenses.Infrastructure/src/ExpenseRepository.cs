using System;
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

        public Task<Category> GetCategoryByIdAsync(int categoryId, CancellationToken ct) {
            return _context.Categories
                .Where(it => it.Id == categoryId)
                .SingleOrDefaultAsync(ct);
        
        }
        public Task<Category> GetCategoryByNameAsync(string categoryName, CancellationToken ct) {
            return _context.Categories
                .Where(it => it.Name == categoryName)
                .SingleOrDefaultAsync(ct);
        }

        public Task LoadExpensesAsync(Category category, CancellationToken ct) {
            Guard.NotNull(category, nameof(category));

            return _context.Entry(category)
                .Collection(it => it.Expenses)
                .LoadAsync(ct);
        }

        public void AddCategory(Category category) {
            Guard.NotNull(category, nameof(category));

            _context.Categories.Add(category);
        }

        public void RemoveCategory(Category category) {
            Guard.NotNull(category, nameof(category));

            _context.Categories.Remove(category);
        }

        public Task<Expense> GetExpenseByIdAsync(int expenseId, CancellationToken ct) {
            return _context.Expenses
                .Where(it => it.Id == expenseId)
                .SingleOrDefaultAsync(ct);
        }

        public void RemoveExpense(Expense expense) {
            Guard.NotNull(expense, nameof(expense));

            _context.Expenses.Remove(expense);
        }

        public Task<Category> GetContainingCategoryAsync(Expense expense, CancellationToken cancellationToken) {
            Guard.NotNull(expense, nameof(expense));

            int categoryId = _context.Entry(expense).Property<int>("CategoryId").CurrentValue;
            return GetCategoryByIdAsync(categoryId, cancellationToken);
        }
    }
}