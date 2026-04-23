using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Route_Fare_Management.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class OperatorFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TourOperators_Code",
                table: "TourOperators");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "TourOperators");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "TourOperators",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_TourOperators_Code",
                table: "TourOperators",
                column: "Code",
                unique: true);
        }
    }
}
