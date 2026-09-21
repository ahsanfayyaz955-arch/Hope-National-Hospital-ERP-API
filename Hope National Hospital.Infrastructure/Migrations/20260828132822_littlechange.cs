using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hope_National_Hospital.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class  Littlechange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Treatments_Appointments_AppointmentId",
                table: "Treatments");

            migrationBuilder.AlterColumn<int>(
                name: "AppointmentId",
                table: "Treatments",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Treatments_Appointments_AppointmentId",
                table: "Treatments",
                column: "AppointmentId",
                principalTable: "Appointments",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Treatments_Appointments_AppointmentId",
                table: "Treatments");

            migrationBuilder.AlterColumn<int>(
                name: "AppointmentId",
                table: "Treatments",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Treatments_Appointments_AppointmentId",
                table: "Treatments",
                column: "AppointmentId",
                principalTable: "Appointments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
