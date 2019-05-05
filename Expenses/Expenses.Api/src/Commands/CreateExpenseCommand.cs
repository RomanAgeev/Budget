using System;

namespace Expenses.Api.Commands {
    public class CreateExpenseCommand : CommandBase {
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }

    }
}