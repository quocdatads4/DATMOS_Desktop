using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DATMOS.Data.Migrations
{
    /// <inheritdoc />
    public partial class CleanMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExamProjects_Exams_ExamListId",
                table: "ExamProjects");

            migrationBuilder.DropForeignKey(
                name: "FK_Exams_ExamSubjects_SubjectId",
                table: "Exams");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Exams",
                table: "Exams");

            migrationBuilder.RenameTable(
                name: "Exams",
                newName: "ExamLists");

            migrationBuilder.RenameIndex(
                name: "IX_Exams_Type_Mode_IsActive",
                table: "ExamLists",
                newName: "IX_ExamLists_Type_Mode_IsActive");

            migrationBuilder.RenameIndex(
                name: "IX_Exams_Type",
                table: "ExamLists",
                newName: "IX_ExamLists_Type");

            migrationBuilder.RenameIndex(
                name: "IX_Exams_SubjectId_IsActive",
                table: "ExamLists",
                newName: "IX_ExamLists_SubjectId_IsActive");

            migrationBuilder.RenameIndex(
                name: "IX_Exams_SubjectId",
                table: "ExamLists",
                newName: "IX_ExamLists_SubjectId");

            migrationBuilder.RenameIndex(
                name: "IX_Exams_Mode",
                table: "ExamLists",
                newName: "IX_ExamLists_Mode");

            migrationBuilder.RenameIndex(
                name: "IX_Exams_IsActive",
                table: "ExamLists",
                newName: "IX_ExamLists_IsActive");

            migrationBuilder.RenameIndex(
                name: "IX_Exams_Difficulty",
                table: "ExamLists",
                newName: "IX_ExamLists_Difficulty");

            migrationBuilder.RenameIndex(
                name: "IX_Exams_CreatedAt",
                table: "ExamLists",
                newName: "IX_ExamLists_CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_Exams_Code",
                table: "ExamLists",
                newName: "IX_ExamLists_Code");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExamLists",
                table: "ExamLists",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "ExamTasks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ExamProjectId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Instructions = table.Column<string>(type: "text", nullable: false),
                    OrderIndex = table.Column<int>(type: "integer", nullable: false),
                    MaxScore = table.Column<double>(type: "float", nullable: false),
                    TaskType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExamTasks_ExamProjects_ExamProjectId",
                        column: x => x.ExamProjectId,
                        principalTable: "ExamProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExamTasks_ExamProjectId",
                table: "ExamTasks",
                column: "ExamProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamTasks_ExamProjectId_OrderIndex",
                table: "ExamTasks",
                columns: new[] { "ExamProjectId", "OrderIndex" });

            migrationBuilder.CreateIndex(
                name: "IX_ExamTasks_OrderIndex",
                table: "ExamTasks",
                column: "OrderIndex");

            migrationBuilder.AddForeignKey(
                name: "FK_ExamLists_ExamSubjects_SubjectId",
                table: "ExamLists",
                column: "SubjectId",
                principalTable: "ExamSubjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ExamProjects_ExamLists_ExamListId",
                table: "ExamProjects",
                column: "ExamListId",
                principalTable: "ExamLists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExamLists_ExamSubjects_SubjectId",
                table: "ExamLists");

            migrationBuilder.DropForeignKey(
                name: "FK_ExamProjects_ExamLists_ExamListId",
                table: "ExamProjects");

            migrationBuilder.DropTable(
                name: "ExamTasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ExamLists",
                table: "ExamLists");

            migrationBuilder.RenameTable(
                name: "ExamLists",
                newName: "Exams");

            migrationBuilder.RenameIndex(
                name: "IX_ExamLists_Type_Mode_IsActive",
                table: "Exams",
                newName: "IX_Exams_Type_Mode_IsActive");

            migrationBuilder.RenameIndex(
                name: "IX_ExamLists_Type",
                table: "Exams",
                newName: "IX_Exams_Type");

            migrationBuilder.RenameIndex(
                name: "IX_ExamLists_SubjectId_IsActive",
                table: "Exams",
                newName: "IX_Exams_SubjectId_IsActive");

            migrationBuilder.RenameIndex(
                name: "IX_ExamLists_SubjectId",
                table: "Exams",
                newName: "IX_Exams_SubjectId");

            migrationBuilder.RenameIndex(
                name: "IX_ExamLists_Mode",
                table: "Exams",
                newName: "IX_Exams_Mode");

            migrationBuilder.RenameIndex(
                name: "IX_ExamLists_IsActive",
                table: "Exams",
                newName: "IX_Exams_IsActive");

            migrationBuilder.RenameIndex(
                name: "IX_ExamLists_Difficulty",
                table: "Exams",
                newName: "IX_Exams_Difficulty");

            migrationBuilder.RenameIndex(
                name: "IX_ExamLists_CreatedAt",
                table: "Exams",
                newName: "IX_Exams_CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_ExamLists_Code",
                table: "Exams",
                newName: "IX_Exams_Code");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Exams",
                table: "Exams",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ExamProjects_Exams_ExamListId",
                table: "ExamProjects",
                column: "ExamListId",
                principalTable: "Exams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Exams_ExamSubjects_SubjectId",
                table: "Exams",
                column: "SubjectId",
                principalTable: "ExamSubjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
