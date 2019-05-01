using Expenses.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Expenses.Infrastructure {
    public class ExpenseCategoryConfiguration : IEntityTypeConfiguration<ExpenseCategory> {
        public void Configure(EntityTypeBuilder<ExpenseCategory> builder) {
            builder.ToTable("expense_categories");

            builder.HasKey(it => it.Id);

            builder.Property<string>(it => it.Name).IsRequired();
            builder.Property<string>(it => it.Description);

            builder.Metadata
                .FindNavigation(nameof(ExpenseCategory.Expenses))
                .SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}