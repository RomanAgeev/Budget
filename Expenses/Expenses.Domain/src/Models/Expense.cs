using System;
using Guards;

namespace Expenses.Domain.Models {
    public class Expense : Entity {
        public Expense(DateTime date, decimal amount, string description) {
            Guard.NotZeroOrNegative(amount, nameof(amount));

            _date = date;
            _amount = amount;
            _description = description;
        }

        DateTime _date;
        decimal _amount;
        string _description;

        public DateTime Date => _date;
        public decimal Amount => _amount;
        public string Description => _description;

        public Expense WithId(int id) {
            Id = id;
            return this;
        }

        public void Update(decimal amount, string description) {
            Guard.NotZeroOrNegative(amount, nameof(amount));
            
            _amount = amount;
            _description = description;
        }
    }
}