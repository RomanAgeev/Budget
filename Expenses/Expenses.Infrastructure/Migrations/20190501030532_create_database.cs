using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Expenses.Infrastructure.Migrations
{
    public partial class create_database : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "expense_categories",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_expense_categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "expenses",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Date = table.Column<DateTime>(nullable: false),
                    Amount = table.Column<decimal>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    ExpenseCategoryId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_expenses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_expenses_expense_categories_ExpenseCategoryId",
                        column: x => x.ExpenseCategoryId,
                        principalTable: "expense_categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "expense_categories",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[] { 1, "Everyday food and drink expenses", "Food" });

            migrationBuilder.InsertData(
                table: "expense_categories",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[] { 2, "Total petrol expenses for each car", "Petrol" });

            migrationBuilder.InsertData(
                table: "expenses",
                columns: new[] { "Id", "Amount", "Date", "Description", "ExpenseCategoryId" },
                values: new object[] { 1, 60m, new DateTime(2019, 4, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Weekly food shopping in the \"Shop & Go\"", 1 });

            migrationBuilder.InsertData(
                table: "expenses",
                columns: new[] { "Id", "Amount", "Date", "Description", "ExpenseCategoryId" },
                values: new object[] { 2, 40m, new DateTime(2019, 4, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), "Weekly food shopping in the \"Shop & Go\"", 1 });

            migrationBuilder.InsertData(
                table: "expenses",
                columns: new[] { "Id", "Amount", "Date", "Description", "ExpenseCategoryId" },
                values: new object[] { 3, 35m, new DateTime(2019, 4, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), "Fillup a fool tank of Ford", 2 });

            migrationBuilder.CreateIndex(
                name: "IX_expenses_ExpenseCategoryId",
                table: "expenses",
                column: "ExpenseCategoryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "expenses");

            migrationBuilder.DropTable(
                name: "expense_categories");
        }
    }
}
