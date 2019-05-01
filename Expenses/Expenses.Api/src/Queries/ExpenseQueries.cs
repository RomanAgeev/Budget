using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using Dapper;
using Guards;

namespace Expenses.Api.Queries {
    public delegate DbConnection DbConnectionFactory();

    public class ExpenseQueries : IExpenseQueries {
        readonly DbConnectionFactory _connectionFactory;

        public ExpenseQueries(DbConnectionFactory connectionFactory) {
            Guard.NotNull(connectionFactory, nameof(connectionFactory));

            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<ExpenseViewModel>> GetExpensesAsync() {
            using(DbConnection connection = _connectionFactory()) {
                connection.Open();

                return await connection.QueryAsync<ExpenseViewModel>(@"
                    SELECT e.Description, c.Name Category, e.Date, e.Amount
                    FROM expenses e
                    INNER JOIN expense_categories c
                    ON e.ExpenseCategoryId = c.Id");
            }
        }
    }
}