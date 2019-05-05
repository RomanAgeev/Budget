using System;
using Expenses.Domain.Models;
using FluentAssertions;
using Xunit;

namespace Expenses.Domain.Tests {
    public class ExpenseTests {
        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public void CreateWithWrongAmountTest(decimal amount) {
            Action create = () => new Expense(new DateTime(2019, 1, 1), amount, null);

            create.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void UpdateTest() {
            var expense = new Expense(new DateTime(2019, 1, 1), 15.5m, "expense_description");

            expense.Date.Should().Be(new DateTime(2019, 1, 1));
            expense.Amount.Should().Be(15.5m);
            expense.Description.Should().Be("expense_description");

            expense.Update(20.1m, "new_expense_description");

            expense.Date.Should().Be(new DateTime(2019, 1, 1));
            expense.Amount.Should().Be(20.1m);
            expense.Description.Should().Be("new_expense_description");
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public void UpdateWithWrongAmountTest(decimal amount) {
            var expense = new Expense(new DateTime(2019, 1, 1), 15.5m, "expense_description");

            expense.Invoking(it => it.Update(amount, "new_expense_description"))
                .Should().Throw<ArgumentException>();
        }
    }
}