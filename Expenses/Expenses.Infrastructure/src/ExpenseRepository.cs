using System.Collections.Generic;
using System.Linq;
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
        public Category GetCategory(int categoryId) {
            return _context.Categories
                .Include(it => it.Expenses)
                .Where(it => it.Id == categoryId)
                .SingleOrDefault();
        }
        public void AddCategory(Category category) {
            Guard.NotNull(category, nameof(category));

            _context.Categories.Add(category);
        }
        public void DeleteCategory(Category category) {
            Guard.NotNull(category, nameof(category));

            _context.Categories.Remove(category);
        }
    }
}