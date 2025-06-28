using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ToDoListManagement.Entity.Migrations
{
    /// <inheritdoc />
    public partial class PermissionsData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PermissionName",
                table: "Permissions",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "PermissionId", "CanAddEdit", "CanDelete", "CanView", "CreatedAt", "CreatedBy", "IsDeleted", "PermissionName", "RoleId", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, true, true, true, new DateTime(2025, 6, 27, 4, 36, 1, 123, DateTimeKind.Utc).AddTicks(7758), null, false, "Projects", 1, null, null },
                    { 2, true, true, true, new DateTime(2025, 6, 27, 4, 36, 1, 123, DateTimeKind.Utc).AddTicks(7782), null, false, "Employees", 1, null, null },
                    { 3, true, true, true, new DateTime(2025, 6, 27, 4, 36, 1, 123, DateTimeKind.Utc).AddTicks(7799), null, false, "Task Board", 1, null, null },
                    { 4, true, true, true, new DateTime(2025, 6, 27, 4, 36, 1, 123, DateTimeKind.Utc).AddTicks(7816), null, false, "Role And Permissions", 1, null, null }
                });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 27, 4, 36, 1, 123, DateTimeKind.Utc).AddTicks(7673));

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "RoleId", "CreatedAt", "CreatedBy", "IsDeleted", "RoleName", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 2, new DateTime(2025, 6, 27, 4, 36, 1, 123, DateTimeKind.Utc).AddTicks(7716), null, false, "Program Manager", null, null },
                    { 3, new DateTime(2025, 6, 27, 4, 36, 1, 123, DateTimeKind.Utc).AddTicks(7735), null, false, "Member", null, null }
                });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "PermissionId", "CanAddEdit", "CanDelete", "CanView", "CreatedAt", "CreatedBy", "IsDeleted", "PermissionName", "RoleId", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 5, false, false, true, new DateTime(2025, 6, 27, 4, 36, 1, 123, DateTimeKind.Utc).AddTicks(7832), null, false, "Projects", 2, null, null },
                    { 6, true, true, true, new DateTime(2025, 6, 27, 4, 36, 1, 123, DateTimeKind.Utc).AddTicks(7850), null, false, "Employees", 2, null, null },
                    { 7, true, true, true, new DateTime(2025, 6, 27, 4, 36, 1, 123, DateTimeKind.Utc).AddTicks(7866), null, false, "Task Board", 2, null, null },
                    { 8, false, false, false, new DateTime(2025, 6, 27, 4, 36, 1, 123, DateTimeKind.Utc).AddTicks(7882), null, false, "Role And Permissions", 2, null, null },
                    { 9, false, false, true, new DateTime(2025, 6, 27, 4, 36, 1, 123, DateTimeKind.Utc).AddTicks(7897), null, false, "Projects", 3, null, null },
                    { 10, false, false, true, new DateTime(2025, 6, 27, 4, 36, 1, 123, DateTimeKind.Utc).AddTicks(7915), null, false, "Employees", 3, null, null },
                    { 11, false, false, true, new DateTime(2025, 6, 27, 4, 36, 1, 123, DateTimeKind.Utc).AddTicks(7931), null, false, "Task Board", 3, null, null },
                    { 12, false, false, false, new DateTime(2025, 6, 27, 4, 36, 1, 123, DateTimeKind.Utc).AddTicks(7946), null, false, "Role And Permissions", 3, null, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "PermissionId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "PermissionId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "PermissionId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "PermissionId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "PermissionId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "PermissionId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "PermissionId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "PermissionId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "PermissionId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "PermissionId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "PermissionId",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "PermissionId",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3);

            migrationBuilder.AlterColumn<string>(
                name: "PermissionName",
                table: "Permissions",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 26, 6, 38, 39, 19, DateTimeKind.Utc).AddTicks(8626));
        }
    }
}
