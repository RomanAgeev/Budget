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
        }
    }
}