namespace Expenses.Domain {
    public enum DomainExceptionCause {
        DuplicatedCategoryName,
        DefaultCategoryUpdateOrDelete,
        CategoryNotFound,
        ExpenseNotFound
    }
}