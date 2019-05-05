using System;
using Expenses.Domain.Models;
using FluentAssertions;
using Xunit;

namespace Expenses.Domain.Tests {
    public class CategoryTests {
        [Theory]
        [InlineData(" ")]
        [InlineData("")]
        [InlineData(null)]
        public void CreateWithWrongNameTest(string name) {
            Action create = () => new Category(name, null);
            create.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void UpdateTest() {
            var category = new Category("category", "category_description");

            category.Name.Should().Be("category");
            category.Description.Should().Be("category_description");

            category.Update("new_category", "new_category_description");

            category.Name.Should().Be("new_category");
            category.Description.Should().Be("new_category_description");

            category.Update("new_new_category", null);

            category.Name.Should().Be("new_new_category");
            category.Description.Should().BeNull();
        }

        [Theory]
        [InlineData(" ")]
        [InlineData("")]
        [InlineData(null)]
        public void UpdateWithWrongNameTest(string name) {
            var category = new Category("category", null);

            category.Invoking(it => it.Update(name, "new_category_description"))
                .Should().Throw<ArgumentException>();
        }

        [Fact]
        public void AddExpenseTest() {
            var category = new Category("category", null);

            var expense1 = category.AddExpense(new DateTime(2019, 1, 1), 100, "expense_1");

            expense1.Date.Should().Be(new DateTime(2019, 1, 1));
            expense1.Amount.Should().Be(100);
            expense1.Description.Should().Be("expense_1");
            category.Expenses.Should().BeEquivalentTo(new[] { expense1 });

            var expense2 = category.AddExpense(new DateTime(2019, 1, 2), 20.5m, null);

            expense2.Date.Should().Be(new DateTime(2019, 1, 2));
            expense2.Amount.Should().Be(20.5m);
            expense2.Description.Should().BeNull();
            category.Expenses.Should().BeEquivalentTo(new[] { expense1, expense2 });
        }

        [Fact]
        public void MoveExpensesTest() {
            var fromCategory = new Category("from_category", null);
            var expense1 = fromCategory.AddExpense(new DateTime(2019, 1, 1), 100, "expense_1");
            var expense2 = fromCategory.AddExpense(new DateTime(2019, 1, 2), 20.5m, null);
            var toCategory = new Category("to_category", null);

            fromCategory.Expenses.Should().BeEquivalentTo(new[] { expense1, expense2 });
            toCategory.Expenses.Should().BeEmpty();

            fromCategory.MoveExpenses(toCategory);

            fromCategory.Expenses.Should().BeEmpty();
            toCategory.Expenses.Should().BeEquivalentTo(new[] { expense1, expense2 });
        }

        [Fact]
        public void MoveExpensesToNullCategoryTest() {
            var fromCategory = new Category("from_category", null);

            fromCategory.Invoking(it => it.MoveExpenses(null))
                .Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void MoveExpenseTest() {
            var fromCategory = new Category("from_category", null);
            var expense1 = fromCategory.AddExpense(new DateTime(2019, 1, 1), 100, "expense_1").WithId(1);
            var expense2 = fromCategory.AddExpense(new DateTime(2019, 1, 2), 20.5m, null).WithId(2);

            var toCategory = new Category("to_category", null);

            fromCategory.Expenses.Should().BeEquivalentTo(new[] { expense1, expense2 });
            toCategory.Expenses.Should().BeEmpty();

            fromCategory.MoveExpense(toCategory, expense1);

            fromCategory.Expenses.Should().BeEquivalentTo(new[] { expense2 });
            toCategory.Expenses.Should().BeEquivalentTo(new[] { expense1 });
        }

        [Fact]
        public void MoveExpenseToNullCategoryTest() {
            var fromCategory = new Category("from_category", null);
            var expense = fromCategory.AddExpense(new DateTime(2019, 1, 2), 20.5m, null).WithId(1);

            fromCategory.Invoking(it => it.MoveExpense(null, expense))
                .Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void MoveNullExpenseTest() {
            var fromCategory = new Category("from_category", null);
            var toCategory = new Category("to_category", null);

            fromCategory.Invoking(it => it.MoveExpense(toCategory, null))
                .Should().Throw<ArgumentException>();
        }

        [Fact]
        public void MoveNotExistExpenseTest() {
            var fromCategory = new Category("from_category", null);
            var expense1 = fromCategory.AddExpense(new DateTime(2019, 1, 1), 100, "expense_1").WithId(1);

            var toCategory = new Category("to_category", null);
            var expense2 = toCategory.AddExpense(new DateTime(2019, 1, 2), 20.5m, null).WithId(2);

            fromCategory.Invoking(it => it.MoveExpense(toCategory, expense2))
                .Should().Throw<InvalidOperationException>();
        }
    }
}