using Expenses.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Expenses.Infrastructure {
    public class CategoryConfiguration : IEntityTypeConfiguration<Category> {
        public void Configure(EntityTypeBuilder<Category> builder) {
            builder.ToTable("Category");

            builder.HasKey(it => it.Id);

            builder.Property<string>(it => it.Name).IsRequired();
            builder.Property<string>(it => it.Description);

            builder.Metadata
                .FindNavigation(nameof(Category.Expenses))
                .SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}