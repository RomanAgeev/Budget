using System;

namespace Expenses.Domain.Models {
    public class Expense : Entity {
        public Expense(DateTime date, decimal amount, string description) {
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
    }
}