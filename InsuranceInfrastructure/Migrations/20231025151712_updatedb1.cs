using Microsoft.EntityFrameworkCore.Migrations;

namespace InsuranceInfrastructure.Migrations
{
    public partial class updatedb1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "AdminAuditLogs",
                nullable: true,
                oldClrType: typeof(long),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "UserId",
                table: "AdminAuditLogs",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
