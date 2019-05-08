using System;
using MediatR;

namespace Expenses.Api.Commands {
    public class CreateExpenseCommand : IRequest<int> {
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }

    }
}