using Microsoft.EntityFrameworkCore.Migrations;

namespace smartpalika.Migrations
{
    public partial class appointment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointment_AspNetUsers_ApplicationUserId",
                table: "Appointment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Appointment",
                table: "Appointment");

            migrationBuilder.RenameTable(
                name: "Appointment",
                newName: "AppointmentUserDetails");

            migrationBuilder.RenameIndex(
                name: "IX_Appointment_ApplicationUserId",
                table: "AppointmentUserDetails",
                newName: "IX_AppointmentUserDetails_ApplicationUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppointmentUserDetails",
                table: "AppointmentUserDetails",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_AppointmentUserDetails_AspNetUsers_ApplicationUserId",
                table: "AppointmentUserDetails",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppointmentUserDetails_AspNetUsers_ApplicationUserId",
                table: "AppointmentUserDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AppointmentUserDetails",
                table: "AppointmentUserDetails");

            migrationBuilder.RenameTable(
                name: "AppointmentUserDetails",
                newName: "Appointment");

            migrationBuilder.RenameIndex(
                name: "IX_AppointmentUserDetails_ApplicationUserId",
                table: "Appointment",
                newName: "IX_Appointment_ApplicationUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Appointment",
                table: "Appointment",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointment_AspNetUsers_ApplicationUserId",
                table: "Appointment",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
