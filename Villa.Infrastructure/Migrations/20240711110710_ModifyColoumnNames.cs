using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Villa.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ModifyColoumnNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UpdatedDate",
                table: "Hotels",
                newName: "Updated_Date");

            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "Hotels",
                newName: "Created_Date");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Updated_Date",
                table: "Hotels",
                newName: "UpdatedDate");

            migrationBuilder.RenameColumn(
                name: "Created_Date",
                table: "Hotels",
                newName: "CreatedDate");
        }
    }
}
