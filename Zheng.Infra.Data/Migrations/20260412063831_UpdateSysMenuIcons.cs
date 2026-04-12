using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zheng.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSysMenuIcons : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "SysMenu",
                keyColumn: "Id",
                keyValue: 1L,
                column: "Icon",
                value: "HomeFilled");

            migrationBuilder.UpdateData(
                table: "SysMenu",
                keyColumn: "Id",
                keyValue: 2L,
                column: "Icon",
                value: "Document");

            migrationBuilder.UpdateData(
                table: "SysMenu",
                keyColumn: "Id",
                keyValue: 3L,
                column: "Icon",
                value: "Setting");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "SysMenu",
                keyColumn: "Id",
                keyValue: 1L,
                column: "Icon",
                value: null);

            migrationBuilder.UpdateData(
                table: "SysMenu",
                keyColumn: "Id",
                keyValue: 2L,
                column: "Icon",
                value: null);

            migrationBuilder.UpdateData(
                table: "SysMenu",
                keyColumn: "Id",
                keyValue: 3L,
                column: "Icon",
                value: null);
        }
    }
}
