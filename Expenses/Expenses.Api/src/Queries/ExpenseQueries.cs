using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Guards;
using Microsoft.Extensions.Logging;

namespace Expenses.Api.Queries {
    public class ExpenseQueries : IExpenseQueries {
        public ExpenseQueries(DbConnectionFactory connectionFactory, ILogger<ExpenseQueries> logger) {
            Guard.NotNull(connectionFactory, nameof(connectionFactory));
            Guard.NotNull(logger, nameof(logger));

            _connectionFactory = connectionFactory;
            _logger = logger;
        }

        readonly DbConnectionFactory _connectionFactory;
        readonly ILogger<ExpenseQueries> _logger;

        public async Task<IEnumerable<ExpenseViewModel>> GetExpensesAsync() {
            _logger.LogInformation("Querying expenses from storage");

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
            Guard.NotZeroOrNegative(expenseId, nameof(expenseId));

            _logger.LogInformation("Querying expense {expenseId} from storage", expenseId);

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