using System;
using System.Collections.Generic;

namespace Expenses.Domain.Models {
    public class Category : Entity, IAggregateRoot {
        public Category(string name, string description) {
            _name = name;
            _description = description;
        }

        string _name;
        string _description;
        readonly List<Expense> _expenses = new List<Expense>();

        public string Name => _name;
        public string Description => _description;
        public IReadOnlyCollection<Expense> Expenses => _expenses.AsReadOnly();

        public void AddExpense(DateTime date, decimal amount, string description) {
            _expenses.Add(new Expense(date, amount, description));
        }

        public void MoveExpenses(Category toCategory) {
            toCategory._expenses.AddRange(_expenses);
            _expenses.Clear();
        }

        public void MoveExense(Category toCategory, Expense expense) {
            toCategory._expenses.Add(expense);
            _expenses.Remove(expense);
        }

        public void Update(string name, string description) {
            _name = name;
            _description = description;
        }
    }    
}