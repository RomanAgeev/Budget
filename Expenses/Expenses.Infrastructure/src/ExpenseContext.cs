using System;
using Expenses.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Expenses.Infrastructure {
    public class ExpenseContext : DbContext {
        public ExpenseContext(DbContextOptions<ExpenseContext> options)
            : base(options) {                
        }
        
        public DbSet<ExpenseCategory> ExpenseCategory { get; set; }
        public DbSet<Expense> Expense { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<ExpenseCategory>()
                .Property(it => it.Name)
                .IsRequired();

            modelBuilder.Entity<Expense>()
                .HasOne(it => it.Category)
                .WithMany(it => it.Expenses)
                .HasForeignKey(it => it.CategoryId);

            SeedData(modelBuilder);
        }

        static void SeedData(ModelBuilder modelBuilder) {
            modelBuilder.Entity<ExpenseCategory>()
                .HasData(
                    new ExpenseCategory {
                        ExpenseCategoryId = 1,
                        Name = "Food",
                        Description = "Everyday food and drink expenses"                    
                    },
                    new ExpenseCategory {
                        ExpenseCategoryId = 2,
                        Name = "Petrol",
                        Description = "Total petrol expenses for each car"
                    });

            modelBuilder.Entity<Expense>()
                .HasData(
                    new Expense {
                        ExpenseId = 1,
                        CategoryId = 1,
                        Date = new DateTime(2019, 4, 21),
                        Amount = 60m,
                        Description = "Weekly food shopping in the \"Shop & Go\"" 
                    },
                    new Expense {
                        ExpenseId = 2,
                        CategoryId = 1,
                        Date = new DateTime(2019, 4, 28),
                        Amount = 40m,
                        Description = "Weekly food shopping in the \"Shop & Go\"" 
                    },
                    new Expense {
                        ExpenseId = 3,
                        CategoryId = 2,
                        Date = new DateTime(2019, 4, 24),
                        Amount = 35m,
                        Description = "Fillup a fool tank of Ford" 
                    }
                );
        }
    }
}