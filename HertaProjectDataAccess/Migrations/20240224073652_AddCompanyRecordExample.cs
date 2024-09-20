using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HertaProjectDataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanyRecordExample : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Companies",
                columns: new[] { "Id", "City", "Name", "PhoneNumber", "PostalCode", "State", "StreetAddress" },
                values: new object[,]
                {
                    { 1, "City 1", "Company 1", "Phone Number 1", "PostalCode 1", "State", "Street Address 1" },
                    { 2, "City 2", "Company 2", "Phone Number 2", "PostalCode 2", "State", "Street Address 2" },
                    { 3, "City 3", "Company 3", "Phone Number 3", "PostalCode 2", "State", "Street Address 3" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
