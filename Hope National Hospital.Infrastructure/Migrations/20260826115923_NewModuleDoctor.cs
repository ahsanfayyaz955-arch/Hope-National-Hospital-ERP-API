using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hope_National_Hospital.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NewModuleDoctor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ConsultationFee",
                table: "Doctor",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Doctor",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConsultationFee",
                table: "Doctor");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Doctor");
        }
    }
}
