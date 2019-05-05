using System;
using System.Collections.Generic;
using System.Linq;
using Guards;

namespace Expenses.Domain.Models {
    public class Category : Entity, IAggregateRoot {
        public Category(string name, string description) {
            Guard.NotNullOrWhiteSpace(name, nameof(name));

            _name = name;
            _description = description;
        }

        string _name;
        string _description;
        readonly List<Expense> _expenses = new List<Expense>();

        public string Name => _name;
        public string Description => _description;
        public IReadOnlyCollection<Expense> Expenses => _expenses.AsReadOnly();

        public Category WithId(int id) {
            Id = id;
            return this;
        }

        public void Update(string name, string description) {
            Guard.NotNullOrWhiteSpace(name, nameof(name));

            _name = name;
            _description = description;
        }

        public Expense AddExpense(DateTime date, decimal amount, string description) {
            var expense = new Expense(date, amount, description);
            _expenses.Add(expense);
            return expense;
        }

        public void MoveExpenses(Category toCategory) {
            Guard.NotNull(toCategory, nameof(toCategory));

            toCategory._expenses.AddRange(_expenses);
            _expenses.Clear();
        }

        public void MoveExpense(Category toCategory, Expense expense) {
            Guard.NotNull(toCategory, nameof(toCategory));
            Guard.NotNull(expense, nameof(expense));

            if(!_expenses.Contains(expense))
                throw new InvalidOperationException();

            toCategory._expenses.Add(expense);
            _expenses.Remove(expense);
        }
    }
}