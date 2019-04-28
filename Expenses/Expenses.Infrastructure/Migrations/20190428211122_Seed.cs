using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Expenses.Infrastructure.Migrations
{
    public partial class Seed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExpenseCategory",
                columns: table => new
                {
                    ExpenseCategoryId = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpenseCategory", x => x.ExpenseCategoryId);
                });

            migrationBuilder.CreateTable(
                name: "Expense",
                columns: table => new
                {
                    ExpenseId = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Amount = table.Column<decimal>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false),
                    CategoryId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Expense", x => x.ExpenseId);
                    table.ForeignKey(
                        name: "FK_Expense_ExpenseCategory_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "ExpenseCategory",
                        principalColumn: "ExpenseCategoryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "ExpenseCategory",
                columns: new[] { "ExpenseCategoryId", "Description", "Name" },
                values: new object[] { 1, "Everyday food and drink expenses", "Food" });

            migrationBuilder.InsertData(
                table: "ExpenseCategory",
                columns: new[] { "ExpenseCategoryId", "Description", "Name" },
                values: new object[] { 2, "Total petrol expenses for each car", "Petrol" });

            migrationBuilder.InsertData(
                table: "Expense",
                columns: new[] { "ExpenseId", "Amount", "CategoryId", "Date", "Description" },
                values: new object[] { 1, 60m, 1, new DateTime(2019, 4, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Weekly food shopping in the \"Shop & Go\"" });

            migrationBuilder.InsertData(
                table: "Expense",
                columns: new[] { "ExpenseId", "Amount", "CategoryId", "Date", "Description" },
                values: new object[] { 2, 40m, 1, new DateTime(2019, 4, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), "Weekly food shopping in the \"Shop & Go\"" });

            migrationBuilder.InsertData(
                table: "Expense",
                columns: new[] { "ExpenseId", "Amount", "CategoryId", "Date", "Description" },
                values: new object[] { 3, 35m, 2, new DateTime(2019, 4, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), "Fillup a fool tank of Ford" });

            migrationBuilder.CreateIndex(
                name: "IX_Expense_CategoryId",
                table: "Expense",
                column: "CategoryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Expense");

            migrationBuilder.DropTable(
                name: "ExpenseCategory");
        }
    }
}
