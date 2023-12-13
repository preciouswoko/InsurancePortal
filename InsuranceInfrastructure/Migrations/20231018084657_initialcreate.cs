using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace InsuranceInfrastructure.Migrations
{
    public partial class initialcreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AdminAuditLogs",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<long>(nullable: true),
                    ServiceName = table.Column<string>(nullable: true),
                    MethodName = table.Column<string>(nullable: true),
                    Parameters = table.Column<string>(nullable: true),
                    ExecutionTime = table.Column<DateTime>(nullable: false),
                    ClientIpAddress = table.Column<string>(nullable: true),
                    Exception = table.Column<string>(nullable: true),
                    BrowserInfo = table.Column<string>(nullable: true),
                    Status = table.Column<bool>(nullable: false),
                    AuditStatusId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminAuditLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AdminRoles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreationTime = table.Column<DateTime>(nullable: true),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DeleterUserId = table.Column<long>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    DisplayName = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    ApprovalTime = table.Column<DateTime>(nullable: true),
                    ApprovalId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationLogs",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    Thread = table.Column<string>(nullable: true),
                    Level = table.Column<string>(nullable: true),
                    Logger = table.Column<string>(nullable: true),
                    Message = table.Column<string>(nullable: true),
                    Exception = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Brokers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BrokerName = table.Column<string>(nullable: true),
                    AccountName = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    CustomerID = table.Column<string>(nullable: true),
                    AccountNumber = table.Column<string>(nullable: true),
                    EmailAddress = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Brokers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RequestID = table.Column<string>(type: "varchar(15)", maxLength: 15, nullable: false),
                    Serial = table.Column<int>(nullable: false),
                    Action = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    Comment = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: false),
                    CommentDate = table.Column<DateTime>(nullable: false),
                    CommentBy = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "EncryptionData",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Key = table.Column<string>(nullable: true),
                    Iv = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EncryptionData", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GatewayLog",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    APIDatetime = table.Column<DateTime>(nullable: false),
                    Source = table.Column<string>(nullable: true),
                    Request = table.Column<string>(nullable: true),
                    Response = table.Column<string>(nullable: true),
                    TransReference = table.Column<string>(nullable: true),
                    RequestTime = table.Column<DateTime>(nullable: false),
                    ResponseTime = table.Column<DateTime>(nullable: false),
                    Endpoint = table.Column<string>(nullable: true),
                    APIType = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GatewayLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InsuranceTable",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RequestID = table.Column<string>(type: "varchar(20)", maxLength: 15, nullable: false),
                    Serial = table.Column<int>(nullable: false),
                    Stage = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    RequestDate = table.Column<DateTime>(nullable: false),
                    RequestType = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true),
                    RequestByUsername = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    RequestByName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    RequestByemail = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false),
                    ToBeAuthroiziedBy = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    AuthorizedDate = table.Column<DateTime>(nullable: true),
                    AuthorizedByUsername = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    AuthorizedByName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    AuthorizedByEmail = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true),
                    PolicyNo = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    PolicyIssuanceDate = table.Column<DateTime>(nullable: true),
                    PolicyExpiryDate = table.Column<DateTime>(nullable: true),
                    PolicyCertificate = table.Column<string>(nullable: true),
                    FileName = table.Column<string>(nullable: true),
                    ContentType = table.Column<string>(nullable: true),
                    CertificateRequestDate = table.Column<DateTime>(nullable: false),
                    CertificateRequestByUsername = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    CertificateRequestByName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    CertificateRequestByemail = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true),
                    CertificateToBeAuthroiziedBy = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    CertificateAuthorizedDate = table.Column<DateTime>(nullable: true),
                    CertificateAuthorizedByUsername = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    CertificateAuthorizedByName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    CertificateAuthorizedByEmail = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true),
                    FEESFTReference = table.Column<string>(type: "varchar(25)", maxLength: 25, nullable: true),
                    COMMFTReference = table.Column<string>(type: "varchar(25)", maxLength: 25, nullable: true),
                    ErrorMessage = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InsuranceTable", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "InsuranceTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InsuranceTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AdminRoleDetails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RoleId = table.Column<int>(nullable: false),
                    MenuOption = table.Column<string>(nullable: true),
                    RoleDescription = table.Column<string>(nullable: true),
                    CreationTime = table.Column<DateTime>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    DeleterUserId = table.Column<long>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminRoleDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdminRoleDetails_AdminRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AdminRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Underwriters",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    EmailAddress = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    BrokerId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Underwriters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Underwriters_Brokers_BrokerId",
                        column: x => x.BrokerId,
                        principalTable: "Brokers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FundTransferLookUp",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TransactionNarration = table.Column<string>(nullable: true),
                    TransactionStatus = table.Column<string>(nullable: true),
                    TransactionRequest = table.Column<string>(nullable: true),
                    TransactionResponse = table.Column<string>(nullable: true),
                    RequesstDate = table.Column<DateTime>(nullable: true),
                    UniqueID = table.Column<string>(nullable: true),
                    TransactionType = table.Column<string>(nullable: true),
                    RequestID = table.Column<string>(nullable: true),
                    InsuranceTableId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FundTransferLookUp", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FundTransferLookUp_InsuranceTable_InsuranceTableId",
                        column: x => x.InsuranceTableId,
                        principalTable: "InsuranceTable",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BrokerInsuranceTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BrokerId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    InsuranceTypeId = table.Column<int>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrokerInsuranceTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BrokerInsuranceTypes_Brokers_BrokerId",
                        column: x => x.BrokerId,
                        principalTable: "Brokers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BrokerInsuranceTypes_InsuranceTypes_InsuranceTypeId",
                        column: x => x.InsuranceTypeId,
                        principalTable: "InsuranceTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InsuranceSubTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    InsuranceTypeId = table.Column<int>(nullable: false),
                    Status = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InsuranceSubTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InsuranceSubTypes_InsuranceTypes_InsuranceTypeId",
                        column: x => x.InsuranceTypeId,
                        principalTable: "InsuranceTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BrokerSubInsuranceTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    BrokerId = table.Column<int>(nullable: false),
                    BrokerInsuranceTypeId = table.Column<int>(nullable: false),
                    PercentageToBank = table.Column<decimal>(nullable: true),
                    Comment = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrokerSubInsuranceTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BrokerSubInsuranceTypes_Brokers_BrokerId",
                        column: x => x.BrokerId,
                        principalTable: "Brokers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BrokerSubInsuranceTypes_BrokerInsuranceTypes_BrokerInsuranceTypeId",
                        column: x => x.BrokerInsuranceTypeId,
                        principalTable: "BrokerInsuranceTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Request",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RequestID = table.Column<string>(type: "varchar(15)", maxLength: 15, nullable: false),
                    AccountNo = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false),
                    AccountName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    Branchcode = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false),
                    CustomerID = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false),
                    CustomerName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    CustomerEmail = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    CollateralValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Premium = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ContractID = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true),
                    BrokerID = table.Column<int>(nullable: false),
                    InsuranceTypeId = table.Column<int>(nullable: false),
                    InsuranceSubTypeID = table.Column<int>(nullable: true),
                    UnderwriterId = table.Column<int>(nullable: true),
                    Status = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    ContractMaturityDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Request", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Request_Brokers_BrokerID",
                        column: x => x.BrokerID,
                        principalTable: "Brokers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Request_BrokerSubInsuranceTypes_InsuranceSubTypeID",
                        column: x => x.InsuranceSubTypeID,
                        principalTable: "BrokerSubInsuranceTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Request_BrokerInsuranceTypes_InsuranceTypeId",
                        column: x => x.InsuranceTypeId,
                        principalTable: "BrokerInsuranceTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Request_Underwriters_UnderwriterId",
                        column: x => x.UnderwriterId,
                        principalTable: "Underwriters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdminRoleDetails_RoleId",
                table: "AdminRoleDetails",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_BrokerInsuranceTypes_BrokerId",
                table: "BrokerInsuranceTypes",
                column: "BrokerId");

            migrationBuilder.CreateIndex(
                name: "IX_BrokerInsuranceTypes_InsuranceTypeId",
                table: "BrokerInsuranceTypes",
                column: "InsuranceTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_BrokerSubInsuranceTypes_BrokerId",
                table: "BrokerSubInsuranceTypes",
                column: "BrokerId");

            migrationBuilder.CreateIndex(
                name: "IX_BrokerSubInsuranceTypes_BrokerInsuranceTypeId",
                table: "BrokerSubInsuranceTypes",
                column: "BrokerInsuranceTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_FundTransferLookUp_InsuranceTableId",
                table: "FundTransferLookUp",
                column: "InsuranceTableId");

            migrationBuilder.CreateIndex(
                name: "IX_InsuranceSubTypes_InsuranceTypeId",
                table: "InsuranceSubTypes",
                column: "InsuranceTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Request_BrokerID",
                table: "Request",
                column: "BrokerID");

            migrationBuilder.CreateIndex(
                name: "IX_Request_InsuranceSubTypeID",
                table: "Request",
                column: "InsuranceSubTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Request_InsuranceTypeId",
                table: "Request",
                column: "InsuranceTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Request_UnderwriterId",
                table: "Request",
                column: "UnderwriterId");

            migrationBuilder.CreateIndex(
                name: "IX_Underwriters_BrokerId",
                table: "Underwriters",
                column: "BrokerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdminAuditLogs");

            migrationBuilder.DropTable(
                name: "AdminRoleDetails");

            migrationBuilder.DropTable(
                name: "ApplicationLogs");

            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "EncryptionData");

            migrationBuilder.DropTable(
                name: "FundTransferLookUp");

            migrationBuilder.DropTable(
                name: "GatewayLog");

            migrationBuilder.DropTable(
                name: "InsuranceSubTypes");

            migrationBuilder.DropTable(
                name: "Request");

            migrationBuilder.DropTable(
                name: "AdminRoles");

            migrationBuilder.DropTable(
                name: "InsuranceTable");

            migrationBuilder.DropTable(
                name: "BrokerSubInsuranceTypes");

            migrationBuilder.DropTable(
                name: "Underwriters");

            migrationBuilder.DropTable(
                name: "BrokerInsuranceTypes");

            migrationBuilder.DropTable(
                name: "Brokers");

            migrationBuilder.DropTable(
                name: "InsuranceTypes");
        }
    }
}
