using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarRental.Migrations
{
    /// <inheritdoc />
    public partial class VerificationChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LicensePath",
                table: "Verifications",
                newName: "LicenseFrontPath");

            migrationBuilder.AddColumn<string>(
                name: "LicenseBackPath",
                table: "Verifications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LicenseBackPath",
                table: "Verifications");

            migrationBuilder.RenameColumn(
                name: "LicenseFrontPath",
                table: "Verifications",
                newName: "LicensePath");
        }
    }
}
