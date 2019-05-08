using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
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
                    SELECT e.Id, c.Id CategoryId, c.Name CategoryName, e.Description, e.Date, e.Amount
                    FROM Expense e
                    INNER JOIN Category c
                    ON e.CategoryId = c.Id");
            }
        }

        public async Task<ExpenseViewModel> GetExpenseAsync(int expenseId) {
            using(DbConnection connection = _connectionFactory()) {
                connection.Open();

                var expenses = await connection.QueryAsync<ExpenseViewModel>($@"
                    SELECT e.Id, c.Id CategoryId, c.Name CategoryName, e.Description, e.Date, e.Amount
                    FROM Expense e
                    INNER JOIN Category c
                    ON e.CategoryId = c.Id
                    WHERE e.Id = {expenseId}");
                
                return expenses.FirstOrDefault();
            }
        }
    }
}