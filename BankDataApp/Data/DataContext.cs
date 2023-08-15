using BankDataApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BankDataApp.Data
{
    public partial class DataContext : DbContext
    {
        public DataContext()
        {
        }
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Payments>(entity =>
            {
                entity.ToTable("payments");

                entity.Property(e => e.Id)
                   .ValueGeneratedOnAdd()
                   .HasColumnName("id");

                entity.Property(e => e.AccountType).HasColumnName("PaymentID");

                entity.Property(e => e.AccountHolder)
                   .HasMaxLength(100)
                   .IsUnicode(false)
                   .HasColumnName("AccountHolder");

                entity.Property(e => e.BranchCode)
                  .HasMaxLength(100)
                  .IsUnicode(false)
                  .HasColumnName("BranchCode");

                entity.Property(e => e.AccountNumber)
               .HasMaxLength(100)
               .IsUnicode(false)
               .HasColumnName("AccountNumber");

                entity.Property(e => e.AccountType).HasColumnName("AccountType");

                entity.Property(e => e.Amount)
                  .HasColumnType("decimal(18, 2)")
                  .HasColumnName("Amount");

                entity.Property(e => e.Status)
                    .HasMaxLength(100)
                   .IsUnicode(false)
                   .HasColumnName("Status");

                entity.Property(e => e.EffectiveStatusDate)
                    .HasColumnType("date")
                    .HasColumnName("EffectiveStatusDate");

                entity.Property(e => e.TransactionDate)
                    .HasColumnType("date")
                    .HasColumnName("TransactionDate");


            });

            OnModelCreatingPartial(modelBuilder);
        }


        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
        public virtual DbSet<Payments> Payments { get; set; } = null!;

    }
}
