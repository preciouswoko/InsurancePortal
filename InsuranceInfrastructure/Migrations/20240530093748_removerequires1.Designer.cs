﻿// <auto-generated />
using System;
using InsuranceInfrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace InsuranceInfrastructure.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20240530093748_removerequires1")]
    partial class removerequires1
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.14-servicing-32113")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("InsuranceCore.Models.AdminAuditLogs", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<Guid>("AuditStatusId");

                    b.Property<string>("BrowserInfo");

                    b.Property<string>("ClientIpAddress");

                    b.Property<string>("Exception");

                    b.Property<DateTime>("ExecutionTime");

                    b.Property<string>("MethodName");

                    b.Property<string>("Parameters");

                    b.Property<string>("ServiceName");

                    b.Property<bool>("Status");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.ToTable("AdminAuditLogs");
                });

            modelBuilder.Entity("InsuranceCore.Models.AdminRoleDetails", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime?>("CreationTime");

                    b.Property<long?>("CreatorUserId");

                    b.Property<long?>("DeleterUserId");

                    b.Property<DateTime?>("DeletionTime");

                    b.Property<bool>("IsDeleted");

                    b.Property<DateTime?>("LastModificationTime");

                    b.Property<long?>("LastModifierUserId");

                    b.Property<string>("MenuOption");

                    b.Property<string>("RoleDescription");

                    b.Property<int>("RoleId");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AdminRoleDetails");
                });

            modelBuilder.Entity("InsuranceCore.Models.AdminRoles", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long?>("ApprovalId");

                    b.Property<DateTime?>("ApprovalTime");

                    b.Property<DateTime?>("CreationTime");

                    b.Property<long?>("CreatorUserId");

                    b.Property<long?>("DeleterUserId");

                    b.Property<DateTime?>("DeletionTime");

                    b.Property<string>("Description");

                    b.Property<string>("DisplayName");

                    b.Property<bool>("IsDeleted");

                    b.Property<DateTime?>("LastModificationTime");

                    b.Property<long?>("LastModifierUserId");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("AdminRoles");
                });

            modelBuilder.Entity("InsuranceCore.Models.ApplicationLogs", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreationTime");

                    b.Property<string>("Exception");

                    b.Property<string>("Level");

                    b.Property<string>("Logger");

                    b.Property<string>("Message");

                    b.Property<string>("Thread");

                    b.HasKey("Id");

                    b.ToTable("ApplicationLogs");
                });

            modelBuilder.Entity("InsuranceCore.Models.Broker", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AccountName");

                    b.Property<string>("AccountNumber");

                    b.Property<string>("BrokerName");

                    b.Property<string>("CustomerID");

                    b.Property<string>("EmailAddress");

                    b.Property<string>("Status");

                    b.HasKey("Id");

                    b.ToTable("Brokers");
                });

            modelBuilder.Entity("InsuranceCore.Models.BrokerInsuranceType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("BrokerId");

                    b.Property<int>("InsuranceTypeId");

                    b.Property<string>("Status")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("BrokerId");

                    b.HasIndex("InsuranceTypeId");

                    b.ToTable("BrokerInsuranceTypes");
                });

            modelBuilder.Entity("InsuranceCore.Models.BrokerSubInsuranceType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("BrokerId");

                    b.Property<int>("BrokerInsuranceTypeId");

                    b.Property<string>("Comment");

                    b.Property<string>("Name");

                    b.Property<decimal?>("PercentageToBank");

                    b.Property<string>("Status")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("BrokerId");

                    b.HasIndex("BrokerInsuranceTypeId");

                    b.ToTable("BrokerSubInsuranceTypes");
                });

            modelBuilder.Entity("InsuranceCore.Models.Comments", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Action")
                        .IsRequired()
                        .HasColumnType("varchar(20)")
                        .HasMaxLength(20);

                    b.Property<string>("Comment")
                        .IsRequired()
                        .HasColumnType("varchar(2000)")
                        .HasMaxLength(2000);

                    b.Property<string>("CommentBy")
                        .IsRequired()
                        .HasColumnType("varchar(150)")
                        .HasMaxLength(150);

                    b.Property<DateTime>("CommentDate");

                    b.Property<string>("RequestID")
                        .IsRequired()
                        .HasColumnType("varchar(15)")
                        .HasMaxLength(15);

                    b.Property<int>("Serial");

                    b.HasKey("ID");

                    b.ToTable("Comments");
                });

            modelBuilder.Entity("InsuranceCore.Models.EncryptionData", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Iv");

                    b.Property<string>("Key");

                    b.HasKey("Id");

                    b.ToTable("EncryptionData");
                });

            modelBuilder.Entity("InsuranceCore.Models.FundTransferLookUp", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long?>("InsuranceTableId");

                    b.Property<DateTime?>("RequesstDate");

                    b.Property<string>("RequestID");

                    b.Property<string>("TransactionNarration");

                    b.Property<string>("TransactionRequest");

                    b.Property<string>("TransactionResponse");

                    b.Property<string>("TransactionStatus");

                    b.Property<string>("TransactionType");

                    b.Property<string>("UniqueID");

                    b.HasKey("Id");

                    b.HasIndex("InsuranceTableId");

                    b.ToTable("FundTransferLookUp");
                });

            modelBuilder.Entity("InsuranceCore.Models.GatewayLog", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("APIDatetime");

                    b.Property<string>("APIType");

                    b.Property<string>("Endpoint");

                    b.Property<string>("Request");

                    b.Property<DateTime>("RequestTime");

                    b.Property<string>("Response");

                    b.Property<DateTime>("ResponseTime");

                    b.Property<string>("Source");

                    b.Property<string>("TransReference");

                    b.HasKey("Id");

                    b.ToTable("GatewayLog");
                });

            modelBuilder.Entity("InsuranceCore.Models.InsuranceSubType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("InsuranceTypeId");

                    b.Property<string>("Name");

                    b.Property<string>("Status")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("InsuranceTypeId");

                    b.ToTable("InsuranceSubTypes");
                });

            modelBuilder.Entity("InsuranceCore.Models.InsuranceTable", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AuthorizedByEmail")
                        .HasColumnType("varchar(150)")
                        .HasMaxLength(150);

                    b.Property<string>("AuthorizedByName")
                        .HasColumnType("varchar(100)")
                        .HasMaxLength(100);

                    b.Property<string>("AuthorizedByUsername")
                        .HasColumnType("varchar(50)")
                        .HasMaxLength(50);

                    b.Property<DateTime?>("AuthorizedDate");

                    b.Property<string>("COMMFTReference")
                        .HasColumnType("varchar(25)")
                        .HasMaxLength(25);

                    b.Property<string>("CertificateAuthorizedByEmail")
                        .HasColumnType("varchar(150)")
                        .HasMaxLength(150);

                    b.Property<string>("CertificateAuthorizedByName")
                        .HasColumnType("varchar(100)")
                        .HasMaxLength(100);

                    b.Property<string>("CertificateAuthorizedByUsername")
                        .HasColumnType("varchar(50)")
                        .HasMaxLength(50);

                    b.Property<DateTime?>("CertificateAuthorizedDate");

                    b.Property<string>("CertificateRequestByName")
                        .HasColumnType("varchar(100)")
                        .HasMaxLength(100);

                    b.Property<string>("CertificateRequestByUsername")
                        .HasColumnType("varchar(50)")
                        .HasMaxLength(50);

                    b.Property<string>("CertificateRequestByemail")
                        .HasColumnType("varchar(150)")
                        .HasMaxLength(150);

                    b.Property<DateTime>("CertificateRequestDate");

                    b.Property<string>("CertificateToBeAuthroiziedBy")
                        .HasColumnType("varchar(50)")
                        .HasMaxLength(50);

                    b.Property<string>("ContentType");

                    b.Property<string>("ErrorMessage");

                    b.Property<string>("FEESFTReference")
                        .HasColumnType("varchar(25)")
                        .HasMaxLength(25);

                    b.Property<string>("FileName");

                    b.Property<string>("PolicyCertificate");

                    b.Property<DateTime?>("PolicyExpiryDate");

                    b.Property<DateTime?>("PolicyIssuanceDate");

                    b.Property<string>("PolicyNo")
                        .HasColumnType("varchar(50)")
                        .HasMaxLength(50);

                    b.Property<string>("RequestByName")
                        .IsRequired()
                        .HasColumnType("varchar(100)")
                        .HasMaxLength(100);

                    b.Property<string>("RequestByUsername")
                        .IsRequired()
                        .HasColumnType("varchar(50)")
                        .HasMaxLength(50);

                    b.Property<string>("RequestByemail")
                        .IsRequired()
                        .HasColumnType("varchar(150)")
                        .HasMaxLength(150);

                    b.Property<DateTime>("RequestDate");

                    b.Property<string>("RequestID")
                        .IsRequired()
                        .HasColumnType("varchar(20)")
                        .HasMaxLength(15);

                    b.Property<string>("RequestType")
                        .HasColumnType("varchar(10)")
                        .HasMaxLength(10);

                    b.Property<int>("Serial");

                    b.Property<string>("Stage")
                        .IsRequired()
                        .HasColumnType("varchar(50)")
                        .HasMaxLength(50);

                    b.Property<string>("Status");

                    b.Property<string>("ToBeAuthroiziedBy")
                        .IsRequired()
                        .HasColumnType("varchar(50)")
                        .HasMaxLength(50);

                    b.HasKey("ID");

                    b.ToTable("InsuranceTable");
                });

            modelBuilder.Entity("InsuranceCore.Models.InsuranceType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name");

                    b.Property<string>("Status")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("InsuranceTypes");
                });

            modelBuilder.Entity("InsuranceCore.Models.Request", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AccountName")
                        .IsRequired()
                        .HasColumnType("varchar(100)")
                        .HasMaxLength(100);

                    b.Property<string>("AccountNo")
                        .IsRequired()
                        .HasColumnType("varchar(10)")
                        .HasMaxLength(10);

                    b.Property<string>("Branchcode")
                        .IsRequired()
                        .HasColumnType("varchar(10)")
                        .HasMaxLength(10);

                    b.Property<int>("BrokerID");

                    b.Property<decimal>("CollateralValue")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("ContractID")
                        .HasColumnType("varchar(20)")
                        .HasMaxLength(20);

                    b.Property<DateTime?>("ContractMaturityDate");

                    b.Property<string>("CustomerEmail")
                        .IsRequired()
                        .HasColumnType("varchar(100)")
                        .HasMaxLength(100);

                    b.Property<string>("CustomerID")
                        .IsRequired()
                        .HasColumnType("varchar(10)")
                        .HasMaxLength(10);

                    b.Property<string>("CustomerName")
                        .IsRequired()
                        .HasColumnType("varchar(100)")
                        .HasMaxLength(100);

                    b.Property<int?>("InsuranceSubTypeID");

                    b.Property<int>("InsuranceTypeId");

                    b.Property<decimal>("Premium")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("RequestID")
                        .IsRequired()
                        .HasColumnType("varchar(15)")
                        .HasMaxLength(15);

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("varchar(20)")
                        .HasMaxLength(20);

                    b.Property<int?>("UnderwriterId");

                    b.Property<decimal>("UpdatedPremium")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("ID");

                    b.HasIndex("BrokerID");

                    b.HasIndex("InsuranceSubTypeID");

                    b.HasIndex("InsuranceTypeId");

                    b.HasIndex("UnderwriterId");

                    b.ToTable("Request");
                });

            modelBuilder.Entity("InsuranceCore.Models.Underwriter", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("BrokerId");

                    b.Property<string>("EmailAddress");

                    b.Property<string>("Name");

                    b.Property<string>("Status");

                    b.HasKey("Id");

                    b.HasIndex("BrokerId");

                    b.ToTable("Underwriters");
                });

            modelBuilder.Entity("InsuranceCore.Models.AdminRoleDetails", b =>
                {
                    b.HasOne("InsuranceCore.Models.AdminRoles", "Role")
                        .WithMany("Roles")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("InsuranceCore.Models.BrokerInsuranceType", b =>
                {
                    b.HasOne("InsuranceCore.Models.Broker", "Broker")
                        .WithMany()
                        .HasForeignKey("BrokerId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("InsuranceCore.Models.InsuranceType", "InsuranceType")
                        .WithMany()
                        .HasForeignKey("InsuranceTypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("InsuranceCore.Models.BrokerSubInsuranceType", b =>
                {
                    b.HasOne("InsuranceCore.Models.Broker", "Broker")
                        .WithMany()
                        .HasForeignKey("BrokerId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("InsuranceCore.Models.BrokerInsuranceType", "InsuranceType")
                        .WithMany()
                        .HasForeignKey("BrokerInsuranceTypeId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("InsuranceCore.Models.FundTransferLookUp", b =>
                {
                    b.HasOne("InsuranceCore.Models.InsuranceTable", "InsuranceTable")
                        .WithMany("fundTransferLookUps")
                        .HasForeignKey("InsuranceTableId");
                });

            modelBuilder.Entity("InsuranceCore.Models.InsuranceSubType", b =>
                {
                    b.HasOne("InsuranceCore.Models.InsuranceType", "InsuranceType")
                        .WithMany("SubInsuranceTypes")
                        .HasForeignKey("InsuranceTypeId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("InsuranceCore.Models.Request", b =>
                {
                    b.HasOne("InsuranceCore.Models.Broker", "Broker")
                        .WithMany()
                        .HasForeignKey("BrokerID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("InsuranceCore.Models.BrokerSubInsuranceType", "InsuranceSubType")
                        .WithMany()
                        .HasForeignKey("InsuranceSubTypeID");

                    b.HasOne("InsuranceCore.Models.BrokerInsuranceType", "InsuranceType")
                        .WithMany()
                        .HasForeignKey("InsuranceTypeId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("InsuranceCore.Models.Underwriter", "Underwriter")
                        .WithMany()
                        .HasForeignKey("UnderwriterId");
                });

            modelBuilder.Entity("InsuranceCore.Models.Underwriter", b =>
                {
                    b.HasOne("InsuranceCore.Models.Broker", "Broker")
                        .WithMany("Underwriters")
                        .HasForeignKey("BrokerId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
