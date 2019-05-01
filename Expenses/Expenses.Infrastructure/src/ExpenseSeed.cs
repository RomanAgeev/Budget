using System;
using Expenses.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Expenses.Infrastructure {
    public static class ExpenseSeed {
        public static void SeedData(this ModelBuilder modelBuilder) {
            modelBuilder.Entity<ExpenseCategory>()
                .HasData(
                    new {
                        Id = 1,
                        Name = "Food",
                        Description = "Everyday food and drink expenses"
                    },
                    new {
                        Id = 2,
                        Name = "Petrol",
                        Description = "Total petrol expenses for each car"
                    });

            modelBuilder.Entity<Expense>()
                .HasData(
                    new {
                        Id = 1,
                        Date = new DateTime(2019, 4, 21),
                        Amount = 60m,
                        Description = "Weekly food shopping in the \"Shop & Go\"",
                        ExpenseCategoryId = 1
                    },
                    new {
                        Id = 2,
                        Date = new DateTime(2019, 4, 28),
                        Amount = 40m,
                        Description = "Weekly food shopping in the \"Shop & Go\"",
                        ExpenseCategoryId = 1
                    },
                    new {
                        Id = 3,
                        Date = new DateTime(2019, 4, 24),
                        Amount = 35m,
                        Description = "Fillup a fool tank of Ford",
                        ExpenseCategoryId = 2
                    });
        }
    }
}