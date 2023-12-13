using InsuranceCore.Models;
using Microsoft.EntityFrameworkCore;

namespace InsuranceInfrastructure.Data
{
    public class ApplicationDbContext :DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BrokerSubInsuranceType>()
        .HasOne(e => e.Broker)
        .WithMany()
        .HasForeignKey(e => e.BrokerId)
        .OnDelete(DeleteBehavior.Restrict); 

            modelBuilder.Entity<BrokerSubInsuranceType>()
                .HasOne(e => e.InsuranceType)
                .WithMany()
                .HasForeignKey(e => e.BrokerInsuranceTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<InsuranceSubType>()
                .HasOne(subInsuranceType => subInsuranceType.InsuranceType)
                .WithMany(insuranceType => insuranceType.SubInsuranceTypes)
                .HasForeignKey(subInsuranceType => subInsuranceType.InsuranceTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Request>()
                .HasOne(r => r.InsuranceType)
                .WithMany()
                .HasForeignKey(r => r.InsuranceTypeId).OnDelete(DeleteBehavior.Restrict); 

        }

        public DbSet<Comments> Comments { get; set; }
        public DbSet<Request> Request { get; set; }
        public DbSet<InsuranceTable> InsuranceTable { get; set; }
        public DbSet<FundTransferLookUp> FundTransferLookUp { get; set; }
        public DbSet<Broker> Brokers { get; set; }
        public DbSet<ApplicationLogs> ApplicationLogs { get; set; }
        public DbSet<Underwriter> Underwriters { get; set; }
        public DbSet<InsuranceSubType> InsuranceSubTypes { get; set; }
        public DbSet<InsuranceType> InsuranceTypes { get; set; }
        public DbSet<AdminAuditLogs> AdminAuditLogs { get; set; }
        public DbSet<AdminRoles> AdminRoles { get; set; }
        public DbSet<AdminRoleDetails> AdminRoleDetails { get; set; }
        public DbSet<GatewayLog> GatewayLog { get; set; }
        public DbSet<EncryptionData> EncryptionData { get; set; }
        public DbSet<BrokerInsuranceType> BrokerInsuranceTypes { get; set; }
        public DbSet<BrokerSubInsuranceType> BrokerSubInsuranceTypes { get; set; }
    }
}
