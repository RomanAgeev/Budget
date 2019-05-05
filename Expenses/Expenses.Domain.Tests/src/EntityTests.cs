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
        [InlineData(-1)]
        [InlineData(0)]
        public void WithWrongId(int id) {
             var entity = CreateEntity();

             entity.Invoking(it => WithId(it, id))
                .Should().Throw<ArgumentException>();
        }

        [Fact]
        public void WithIdMoreThanOnceTest() {
            var entity = CreateEntity();

            WithId(entity, 1)
                .Invoking(it => WithId(it, 2))
                .Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void EqualsWithSameIdTest() {
            var entity1 = CreateEntity();
            var entity2 = CreateEntity();

            entity1.Should().NotBe(entity2);

            entity1 = WithId(entity1, 1);
            entity2 = WithId(entity2, 1);

            entity1.Should().Be(entity2);
        }

        [Fact]
        public void EqualsWithDifferentIdTest() {
            var entity1 = CreateEntity();
            var entity2 = CreateEntity();

            entity1.Should().NotBe(entity2);

            entity1 = WithId(entity1, 1);
            entity2 = WithId(entity2, 2);

            entity1.Should().NotBe(entity2);
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