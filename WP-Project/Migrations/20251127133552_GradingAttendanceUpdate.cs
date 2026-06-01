using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace WPProject.Migrations
{
    /// <inheritdoc />
    public partial class GradingAttendanceUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "AssignmentScore",
                table: "Enrollments",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "FinalScore",
                table: "Enrollments",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "MidScore",
                table: "Enrollments",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "ProjectScore",
                table: "Enrollments",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "QuizScore",
                table: "Enrollments",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "TotalScore",
                table: "Enrollments",
                type: "double precision",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Attendances",
                columns: table => new
                {
                    AttendanceId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CourseOfferingId = table.Column<int>(type: "integer", nullable: false),
                    StudentId = table.Column<int>(type: "integer", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsPresent = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attendances", x => x.AttendanceId);
                    table.ForeignKey(
                        name: "FK_Attendances_CourseOfferings_CourseOfferingId",
                        column: x => x.CourseOfferingId,
                        principalTable: "CourseOfferings",
                        principalColumn: "CourseOfferingId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Attendances_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "StudentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_CourseOfferingId",
                table: "Attendances",
                column: "CourseOfferingId");

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_StudentId",
                table: "Attendances",
                column: "StudentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Attendances");

            migrationBuilder.DropColumn(
                name: "AssignmentScore",
                table: "Enrollments");

            migrationBuilder.DropColumn(
                name: "FinalScore",
                table: "Enrollments");

            migrationBuilder.DropColumn(
                name: "MidScore",
                table: "Enrollments");

            migrationBuilder.DropColumn(
                name: "ProjectScore",
                table: "Enrollments");

            migrationBuilder.DropColumn(
                name: "QuizScore",
                table: "Enrollments");

            migrationBuilder.DropColumn(
                name: "TotalScore",
                table: "Enrollments");
        }
    }
}
