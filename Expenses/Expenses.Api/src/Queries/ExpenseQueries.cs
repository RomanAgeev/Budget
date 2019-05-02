using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using Dapper;
using Guards;

namespace Expenses.Api.Queries {
    public class ExpenseQueries : IExpenseQueries {
        public ExpenseQueries(DbConnectionFactory connectionFactory) {
            Guard.NotNull(connectionFactory, nameof(connectionFactory));

            _connectionFactory = connectionFactory;
        }

        readonly DbConnectionFactory _connectionFactory;

        public async Task<IEnumerable<ExpenseViewModel>> GetExpensesAsync() {
            using(DbConnection connection = _connectionFactory()) {
                connection.Open();

                return await connection.QueryAsync<ExpenseViewModel>(@"
                    SELECT e.Description, c.Name Category, e.Date, e.Amount
                    FROM Expense e
                    INNER JOIN Category c
                    ON e.CategoryId = c.Id");
            }
        }
    }
}