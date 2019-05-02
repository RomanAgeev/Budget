using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Expenses.Domain;
using Expenses.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Expenses.Infrastructure {
    public class ExpenseContext : DbContext, IUnitOfWork {
        public ExpenseContext(DbContextOptions<ExpenseContext> options)
            : base(options) {                
        }        

        public DbSet<ExpenseCategory> ExpenseCategories { get; set; }
        public DbSet<Expense> Expenses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.ApplyConfiguration(new ExpenseCategoryConfiguration());
            modelBuilder.ApplyConfiguration(new ExpenseConfiguration());

            modelBuilder.SeedData();
        }        
    }
}