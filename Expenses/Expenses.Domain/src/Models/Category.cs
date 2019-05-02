using System;
using System.Collections.Generic;
using Expenses.Domain.Common;

namespace Expenses.Domain.Models {
    public class Category : Entity, IAggregateRoot {
        public Category(string name, string description) {
            _name = name;
            _description = description;
        }

        string _name;
        string _description;
        List<Expense> _expenses = new List<Expense>();

        public string Name => _name;
        public string Description => _description;
        public IReadOnlyCollection<Expense> Expenses => _expenses.AsReadOnly();

        public void AddExpense(DateTime date, decimal amount, string description) {
            _expenses.Add(new Expense(date, amount, description));
        }
    }    
}