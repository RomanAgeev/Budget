using System.Collections.Generic;
using Expenses.Domain.Models;

namespace Expenses.Domain {
    public interface IExpenseRepository {
        IUnitOfWork UnitOfWork { get; }

        IEnumerable<Category> GetCategories();
        Category GetCategory(int categoryId);
        void AddCategory(Category category);
        void DeleteCategory(Category category);
    }
}