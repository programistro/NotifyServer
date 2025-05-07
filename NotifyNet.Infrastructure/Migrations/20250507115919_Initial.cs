using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NotifyNet.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "_Employees",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    UserName = table.Column<string>(type: "text", nullable: false),
                    IsCollectiveAccount = table.Column<bool>(type: "boolean", nullable: true),
                    ServiceNumber = table.Column<string>(type: "text", nullable: false),
                    PostId = table.Column<Guid>(type: "uuid", nullable: true),
                    DivisionId = table.Column<Guid>(type: "uuid", nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DateOfEmployment = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    GenderId = table.Column<Guid>(type: "uuid", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Photo = table.Column<string>(type: "text", nullable: false),
                    CooperationId = table.Column<Guid>(type: "uuid", nullable: true),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Employees", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderNumber = table.Column<int>(type: "integer", nullable: true),
                    EmployeeApplicantId = table.Column<Guid>(type: "uuid", nullable: true),
                    BuildingId = table.Column<Guid>(type: "uuid", nullable: true),
                    DivisionId = table.Column<Guid>(type: "uuid", nullable: true),
                    EquipmentId = table.Column<Guid>(type: "uuid", nullable: true),
                    EventId = table.Column<Guid>(type: "uuid", nullable: true),
                    ProcessId = table.Column<Guid>(type: "uuid", nullable: true),
                    RecordId = table.Column<Guid>(type: "uuid", nullable: true),
                    EmployeeDispatcherId = table.Column<Guid>(type: "uuid", nullable: true),
                    DescriptionDispathcer = table.Column<string>(type: "text", nullable: false),
                    EmployeeNotificationId = table.Column<Guid>(type: "uuid", nullable: true),
                    PriorityId = table.Column<Guid>(type: "uuid", nullable: true),
                    DateModeration = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EmployeeExecuterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DateOfExecution = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DescriptionOfWork = table.Column<string>(type: "text", nullable: false),
                    DateWorkStatus = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DateOfClose = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders__Employees_EquipmentId",
                        column: x => x.EquipmentId,
                        principalTable: "_Employees",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_EquipmentId",
                table: "Orders",
                column: "EquipmentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "_Employees");
        }
    }
}
