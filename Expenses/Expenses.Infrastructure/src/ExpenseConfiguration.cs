using System;
using Expenses.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Expenses.Infrastructure {
    public class ExpenseConfiguration : IEntityTypeConfiguration<Expense> {
        public void Configure(EntityTypeBuilder<Expense> builder) {
            builder.ToTable("expenses");

            builder.HasKey(it => it.Id);

            builder.Property<DateTime>(it => it.Date).IsRequired();
            builder.Property<decimal>(it => it.Amount).IsRequired();
            builder.Property<string>(it => it.Description);

            builder.Property<int>("ExpenseCategoryId").IsRequired();
        }
    }
}