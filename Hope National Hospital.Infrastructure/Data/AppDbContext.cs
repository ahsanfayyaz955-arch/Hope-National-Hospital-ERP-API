using Hope_National_Hospital.Domain.Entities;
using Hope_National_Hospital.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Hope_National_Hospital.Infrastructure.Data
{
    public class AppDbContext:IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options ) : base(options)
        {
          
        }

        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Treatment> Treatments { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceItem> InvoiceItems { get; set; }

        public DbSet<Receptionist> Receptionist { get; set; }

        public DbSet<Doctor> Doctor { get; set; }

        public DbSet<Payment> Payments { get; set; }

        public DbSet<Room> Rooms { get; set; }
        public DbSet<Bed> Beds { get; set; }

        public DbSet<Admission> Admissions { get; set; }


        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Payments 
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Invoice)
                .WithMany(i => i.Payments)
                .HasForeignKey(p => p.InvoiceId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Patient)
                .WithMany().HasForeignKey(p => p.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasColumnType("decimal(18,2)");

            //Departments And Receptionist 
            modelBuilder.Entity<Receptionist>()
            .HasOne(r => r.Department)
            .WithMany(d => d.receptionists)
            .HasForeignKey(r => r.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

            // Admissions 
            modelBuilder.Entity<Admission>()
             .HasOne(a => a.Patient)
            .WithMany(p => p.Admissions)
            .HasForeignKey(a => a.PatientId)
           .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Admission>()
                .HasOne(a => a.Doctor)
                .WithMany()
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Admission>()
                .HasOne(a => a.Bed)
                .WithMany()
                .HasForeignKey(a => a.BedId)
                .OnDelete(DeleteBehavior.Restrict);

            //Appointment RelationShips 
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Doctor)
                .WithMany(d => d.Appointments)
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            //Patients And Appointments 
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Patient)
                .WithMany(p => p.Appointment)
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            //Receptionist And Appointments 
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.BookedByReceptionist)
                .WithMany(r => r.Appointments)
                .HasForeignKey(a => a.ReceptionistId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            // bed and room
            modelBuilder.Entity<Room>()
                .HasOne(r => r.Department)
                .WithMany()
                .HasForeignKey(r => r.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Bed>()
                .HasOne(r => r.Room)
                .WithMany(r => r.Beds)
                .HasForeignKey(b => b.RoomId)
                .OnDelete(DeleteBehavior.Restrict);
           
            /// Treatment And Doctor 
            modelBuilder.Entity<Treatment>()
           .HasOne(t => t.Doctor)
           .WithMany()
           .HasForeignKey(t => t.DoctorId)
           .OnDelete(DeleteBehavior.Restrict);

            // Treatment & Patient (One-to-Many)
            modelBuilder.Entity<Treatment>()
                .HasOne(t => t.Patient)
                .WithMany()
                .HasForeignKey(t => t.PatinetId)
                .OnDelete(DeleteBehavior.Restrict);

            // --- Invoice & InvoiceItems (One-to-Many Cascade) ---
            modelBuilder.Entity<InvoiceItem>()
               .HasOne(i => i.Invoice)
               .WithMany(i => i.InvoiceItems)
              .HasForeignKey(i => i.InvoiceId)
              .OnDelete(DeleteBehavior.Cascade);
                
                
            // decimal precition settings 

            modelBuilder.Entity<Treatment>()
               .Property(t => t.Cost)
               .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Invoice>()
                .Property(i => i.GrossAmount).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Invoice>()
                .Property(i => i.DiscountAmount).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Invoice>()
                .Property(i => i.NetAmount).HasColumnType("decimal(18,2)");

            modelBuilder.Entity<InvoiceItem>()
                .Property(item => item.UnitPrice).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<InvoiceItem>()
                .Property(item => item.TotalPrice).HasColumnType("decimal(18,2)");
        }
    }
}
