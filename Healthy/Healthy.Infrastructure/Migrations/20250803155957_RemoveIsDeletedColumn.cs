using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Healthy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveIsDeletedColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_IsDeleted",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Categories_IsDeleted",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "UserRoles");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Meals");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Exercises");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Diaries");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Columns");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "BodyRecords");

            migrationBuilder.CreateIndex(
                name: "IX_Users_DeletedAt",
                table: "Users",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_DeletedAt",
                table: "UserRoles",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Meals_DeletedAt",
                table: "Meals",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Exercises_DeletedAt",
                table: "Exercises",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Diaries_DeletedAt",
                table: "Diaries",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Columns_DeletedAt",
                table: "Columns",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_DeletedAt",
                table: "Categories",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_BodyRecords_DeletedAt",
                table: "BodyRecords",
                column: "DeletedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_DeletedAt",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_UserRoles_DeletedAt",
                table: "UserRoles");

            migrationBuilder.DropIndex(
                name: "IX_Meals_DeletedAt",
                table: "Meals");

            migrationBuilder.DropIndex(
                name: "IX_Exercises_DeletedAt",
                table: "Exercises");

            migrationBuilder.DropIndex(
                name: "IX_Diaries_DeletedAt",
                table: "Diaries");

            migrationBuilder.DropIndex(
                name: "IX_Columns_DeletedAt",
                table: "Columns");

            migrationBuilder.DropIndex(
                name: "IX_Categories_DeletedAt",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_BodyRecords_DeletedAt",
                table: "BodyRecords");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "UserRoles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Meals",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Exercises",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Diaries",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Columns",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Categories",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "BodyRecords",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "BodyRecords",
                keyColumn: "Id",
                keyValue: new Guid("11111111-bbbb-bbbb-bbbb-111111111111"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "BodyRecords",
                keyColumn: "Id",
                keyValue: new Guid("22222222-bbbb-bbbb-bbbb-222222222222"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "BodyRecords",
                keyColumn: "Id",
                keyValue: new Guid("33333333-bbbb-bbbb-bbbb-333333333333"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "BodyRecords",
                keyColumn: "Id",
                keyValue: new Guid("44444444-bbbb-bbbb-bbbb-444444444444"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "BodyRecords",
                keyColumn: "Id",
                keyValue: new Guid("55555555-bbbb-bbbb-bbbb-555555555555"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "BodyRecords",
                keyColumn: "Id",
                keyValue: new Guid("66666666-bbbb-bbbb-bbbb-666666666666"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "BodyRecords",
                keyColumn: "Id",
                keyValue: new Guid("77777777-bbbb-bbbb-bbbb-777777777777"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("111111aa-ccaa-ccaa-ccaa-111111111111"),
                columns: new string[0],
                values: new object[0]);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("222222aa-ccaa-ccaa-ccaa-222222222222"),
                columns: new string[0],
                values: new object[0]);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("333333aa-ccaa-ccaa-ccaa-333333333333"),
                columns: new string[0],
                values: new object[0]);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("444444aa-ccaa-ccaa-ccaa-444444444444"),
                columns: new string[0],
                values: new object[0]);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("555555aa-ccaa-ccaa-ccaa-555555555555"),
                columns: new string[0],
                values: new object[0]);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("666666aa-ccaa-ccaa-ccaa-666666666666"),
                columns: new string[0],
                values: new object[0]);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("777777aa-ccaa-ccaa-ccaa-777777777777"),
                columns: new string[0],
                values: new object[0]);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("888888aa-ccaa-ccaa-ccaa-888888888888"),
                columns: new string[0],
                values: new object[0]);

            migrationBuilder.UpdateData(
                table: "Columns",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-cccc-cccc-cccc-cccccccccccc"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "Columns",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-cccc-cccc-cccc-cccccccccccc"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "Columns",
                keyColumn: "Id",
                keyValue: new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "Columns",
                keyColumn: "Id",
                keyValue: new Guid("dddddddd-cccc-cccc-cccc-cccccccccccc"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "Columns",
                keyColumn: "Id",
                keyValue: new Guid("eeeeeeee-cccc-cccc-cccc-cccccccccccc"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "Diaries",
                keyColumn: "Id",
                keyValue: new Guid("11111111-dddd-dddd-dddd-111111111111"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "Diaries",
                keyColumn: "Id",
                keyValue: new Guid("22222222-dddd-dddd-dddd-222222222222"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "Diaries",
                keyColumn: "Id",
                keyValue: new Guid("33333333-dddd-dddd-dddd-333333333333"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "Diaries",
                keyColumn: "Id",
                keyValue: new Guid("44444444-dddd-dddd-dddd-444444444444"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "Diaries",
                keyColumn: "Id",
                keyValue: new Guid("55555555-dddd-dddd-dddd-555555555555"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "Diaries",
                keyColumn: "Id",
                keyValue: new Guid("66666666-dddd-dddd-dddd-666666666666"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "Diaries",
                keyColumn: "Id",
                keyValue: new Guid("77777777-dddd-dddd-dddd-777777777777"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "Diaries",
                keyColumn: "Id",
                keyValue: new Guid("88888888-dddd-dddd-dddd-888888888888"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("11111111-eeee-eeee-eeee-111111111111"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("22222222-eeee-eeee-eeee-222222222222"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("33333333-eeee-eeee-eeee-333333333333"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("44444444-eeee-eeee-eeee-444444444444"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("55555555-eeee-eeee-eeee-555555555555"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("66666666-eeee-eeee-eeee-666666666666"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("77777777-eeee-eeee-eeee-777777777777"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "Meals",
                keyColumn: "Id",
                keyValue: new Guid("11111111-bbbb-bbbb-aaaa-111111111111"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "Meals",
                keyColumn: "Id",
                keyValue: new Guid("22222222-bbbb-bbbb-aaaa-222222222222"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "Meals",
                keyColumn: "Id",
                keyValue: new Guid("33333333-bbbb-bbbb-aaaa-333333333333"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "Meals",
                keyColumn: "Id",
                keyValue: new Guid("44444444-bbbb-bbbb-aaaa-444444444444"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "Meals",
                keyColumn: "Id",
                keyValue: new Guid("55555555-bbbb-bbbb-aaaa-555555555555"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "Meals",
                keyColumn: "Id",
                keyValue: new Guid("66666666-bbbb-bbbb-aaaa-666666666666"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "Meals",
                keyColumn: "Id",
                keyValue: new Guid("77777777-bbbb-bbbb-aaaa-777777777777"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "Meals",
                keyColumn: "Id",
                keyValue: new Guid("88888888-bbbb-bbbb-aaaa-888888888888"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "Meals",
                keyColumn: "Id",
                keyValue: new Guid("99999999-bbbb-bbbb-aaaa-999999999999"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: new Guid("11111111-aaaa-aaaa-aaaa-111111111111"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: new Guid("22222222-bbbb-bbbb-bbbb-222222222222"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                columns: new string[0],
                values: new object[0]);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                columns: new string[0],
                values: new object[0]);

            migrationBuilder.CreateIndex(
                name: "IX_Users_IsDeleted",
                table: "Users",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_IsDeleted",
                table: "Categories",
                column: "IsDeleted");
        }
    }
}
