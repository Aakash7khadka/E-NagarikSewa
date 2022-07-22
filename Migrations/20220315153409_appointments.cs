using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace smartpalika.Migrations
{
    public partial class appointments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.CreateTable(
                name: "Appointment_All",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServiceType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Date = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    priority = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Time = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Provider = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    isAvailable = table.Column<bool>(type: "bit", nullable: false),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appointment_All", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Appointment_All_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Appointment_All_ApplicationUserId",
                table: "Appointment_All",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointment_AspNetUsers_ApplicationUserId",
                table: "Appointment",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointment_AspNetUsers_ApplicationUserId",
                table: "Appointment");

            migrationBuilder.DropTable(
                name: "Appointment_All");

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
    }
}
