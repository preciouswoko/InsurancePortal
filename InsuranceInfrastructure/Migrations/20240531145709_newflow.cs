using Microsoft.EntityFrameworkCore.Migrations;

namespace InsuranceInfrastructure.Migrations
{
    public partial class newflow : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Request_BrokerSubInsuranceTypes_InsuranceSubTypeID",
                table: "Request");

            migrationBuilder.DropForeignKey(
                name: "FK_Request_BrokerInsuranceTypes_InsuranceTypeId",
                table: "Request");

            migrationBuilder.AddForeignKey(
                name: "FK_Request_InsuranceSubTypes_InsuranceSubTypeID",
                table: "Request",
                column: "InsuranceSubTypeID",
                principalTable: "InsuranceSubTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Request_InsuranceTypes_InsuranceTypeId",
                table: "Request",
                column: "InsuranceTypeId",
                principalTable: "InsuranceTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Request_InsuranceSubTypes_InsuranceSubTypeID",
                table: "Request");

            migrationBuilder.DropForeignKey(
                name: "FK_Request_InsuranceTypes_InsuranceTypeId",
                table: "Request");

            migrationBuilder.AddForeignKey(
                name: "FK_Request_BrokerSubInsuranceTypes_InsuranceSubTypeID",
                table: "Request",
                column: "InsuranceSubTypeID",
                principalTable: "BrokerSubInsuranceTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Request_BrokerInsuranceTypes_InsuranceTypeId",
                table: "Request",
                column: "InsuranceTypeId",
                principalTable: "BrokerInsuranceTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
