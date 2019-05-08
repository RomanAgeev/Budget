using System;
using Expenses.Domain.Models;
using FluentAssertions;
using Xunit;

namespace Expenses.Domain.Tests {
    public abstract class EntityTests<TEntity> where TEntity : Entity {
        [Fact]
        public void WithIdTest() {
            var entity = CreateEntity();

            entity.Id.Should().Be(0);

            WithId(entity, 1).Id.Should().Be(1);
        }

        [Theory]
        [InlineData(-1, true)]
        [InlineData(0, true)]
        [InlineData(1, false)]
        public void IsTransientTest(int id, bool expectedIsTransient) {
            var entiry = CreateEntity();
            WithId(entiry, id).IsTransient.Should().Be(expectedIsTransient);
        }

        [Theory]
        [InlineData(-2, -1, false)]
        [InlineData(-1, 0, false)]
        [InlineData(0, 1, false)]
        [InlineData(1, 2, false)]
        [InlineData(-1, -1, false)]
        [InlineData(0, 0, false)]
        [InlineData(1, 1, true)]
        public void EqualsTest(int id1, int id2, bool expectedEquals) {
            var entity1 = CreateEntity();
            var entity2 = CreateEntity();

            WithId(entity1, id1);
            WithId(entity2, id2);

            entity1.Equals(entity2).Should().Be(expectedEquals);
        }

        protected abstract TEntity CreateEntity();
        protected abstract TEntity WithId(TEntity entity, int id);
    }

    public class CategoryEntityTests : EntityTests<Category> {
        protected override Category CreateEntity() {
            return new Category("category", null);            
        }

        protected override Category WithId(Category entity, int id) {
            return entity.WithId(id);
        }
    }

    public class ExpenseEntityTests : EntityTests<Expense> {
        protected override Expense CreateEntity() {
            return new Expense(new DateTime(2019, 1, 1), 100, null);
        }

        protected override Expense WithId(Expense entity, int id) {
            return entity.WithId(id);
        }
    }
}