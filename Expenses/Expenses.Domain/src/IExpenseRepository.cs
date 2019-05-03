using System.Collections.Generic;
using Expenses.Domain.Models;

namespace Expenses.Domain {
    public interface IExpenseRepository {
        IUnitOfWork UnitOfWork { get; }

        IEnumerable<Category> GetCategories();
        void AddCategory(Category category);
    }
}