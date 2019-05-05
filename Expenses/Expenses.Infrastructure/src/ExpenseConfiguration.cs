using System;
using Expenses.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Expenses.Infrastructure {
    public class ExpenseConfiguration : IEntityTypeConfiguration<Expense> {
        public void Configure(EntityTypeBuilder<Expense> builder) {
            builder.ToTable("Expense");

            builder.HasKey(it => it.Id);
            
            builder.Ignore("IsTransient");

            builder.Property<DateTime>(it => it.Date).IsRequired();
            builder.Property<decimal>(it => it.Amount).IsRequired();
            builder.Property<string>(it => it.Description);

            builder.Property<int>("CategoryId").IsRequired();
        }
    }
}