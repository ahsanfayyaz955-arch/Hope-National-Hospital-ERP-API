using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hope_National_Hospital.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SomeEnitityChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Diagnosis",
                table: "Treatments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Treatments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TreatmentDate",
                table: "Treatments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Diagnosis",
                table: "Treatments");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Treatments");

            migrationBuilder.DropColumn(
                name: "TreatmentDate",
                table: "Treatments");
        }
    }
}
