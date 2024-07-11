using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Villa.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class seedDataToHotel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Hotels",
                columns: new[] { "Id", "Area", "Created_Date", "Description", "ImageUrl", "Name", "Occupancy", "Price", "Updated_Date" },
                values: new object[,]
                {
                    { 1, 60, null, "Hotel...", "https://placehold.co/600x400", "Royal Hotel", 2, 200.0, null },
                    { 2, 80, null, "Hotely...", "https://placehold.co/600x401", "Premium Hotel", 4, 300.0, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Hotels",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Hotels",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
