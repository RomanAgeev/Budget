namespace Expenses.Api.Commands {
    public class CreateCategoryCommand : CommandBase {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}