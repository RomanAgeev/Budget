using Expenses.Domain;
using Expenses.Domain.Models;
using Guards;

namespace Expenses.Infrastructure {
    public class ExpenseRepository : IExpenseRepository {
        public ExpenseRepository(ExpenseContext context) {
            Guard.NotNull(context, nameof(context));

            _context = context;
        }

        readonly ExpenseContext _context;

        public IUnitOfWork UnitOfWork => _context;

        public void AddCategory(Category category) {
            Guard.NotNull(category, nameof(category));

            _context.Categories.Add(category);
        }
    }
}