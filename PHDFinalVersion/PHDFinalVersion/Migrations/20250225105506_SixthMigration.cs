using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PHDFinalVersion.Migrations
{
    /// <inheritdoc />
    public partial class SixthMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StudentToStudent",
                columns: table => new
                {
                    StudentID1 = table.Column<int>(type: "int", nullable: false),
                    StudentID2 = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentToStudent", x => new { x.StudentID1, x.StudentID2 });
                    table.ForeignKey(
                        name: "FK_StudentToStudent_StudentProfile_StudentID1",
                        column: x => x.StudentID1,
                        principalTable: "StudentProfile",
                        principalColumn: "StudentID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentToStudent_StudentProfile_StudentID2",
                        column: x => x.StudentID2,
                        principalTable: "StudentProfile",
                        principalColumn: "StudentID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudentToStudent_StudentID2",
                table: "StudentToStudent",
                column: "StudentID2");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentToStudent");
        }
    }
}
