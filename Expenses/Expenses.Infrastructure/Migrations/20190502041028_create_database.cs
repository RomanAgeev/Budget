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
                name: "Category",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Expense",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Date = table.Column<DateTime>(nullable: false),
                    Amount = table.Column<decimal>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    CategoryId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Expense", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Expense_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Category",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[] { 1, "Everyday food and drink expenses", "Food" });

            migrationBuilder.InsertData(
                table: "Category",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[] { 2, "Total petrol expenses for each car", "Petrol" });

            migrationBuilder.InsertData(
                table: "Expense",
                columns: new[] { "Id", "Amount", "CategoryId", "Date", "Description" },
                values: new object[] { 1, 60m, 1, new DateTime(2019, 4, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Weekly food shopping in the \"Shop & Go\"" });

            migrationBuilder.InsertData(
                table: "Expense",
                columns: new[] { "Id", "Amount", "CategoryId", "Date", "Description" },
                values: new object[] { 2, 40m, 1, new DateTime(2019, 4, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), "Weekly food shopping in the \"Shop & Go\"" });

            migrationBuilder.InsertData(
                table: "Expense",
                columns: new[] { "Id", "Amount", "CategoryId", "Date", "Description" },
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
                name: "Category");
        }
    }
}
