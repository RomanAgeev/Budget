namespace Expenses.Api.Commands {
    public class UpdateCategoryCommand : CommandBase {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}