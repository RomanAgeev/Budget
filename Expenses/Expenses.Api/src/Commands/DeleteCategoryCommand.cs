namespace Expenses.Api.Commands {
    public class DeleteCategoryCommand : CommandBase {
        public int CategoryId { get; set; }
    }
}