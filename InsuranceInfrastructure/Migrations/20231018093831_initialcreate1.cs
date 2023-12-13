using Microsoft.EntityFrameworkCore.Migrations;

namespace InsuranceInfrastructure.Migrations
{
    public partial class initialcreate1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "BrokerInsuranceTypes");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "BrokerInsuranceTypes",
                nullable: false,
                defaultValue: false);
        }
    }
}
