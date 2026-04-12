using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zheng.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAdminRoleName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: new Guid("c69cd53c-0ced-4aae-a120-1a7097e43650"),
                column: "Name",
                value: "系統管理員");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: new Guid("c69cd53c-0ced-4aae-a120-1a7097e43650"),
                column: "Name",
                value: "Admin");
        }
    }
}
