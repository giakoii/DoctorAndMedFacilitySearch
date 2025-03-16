using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DataAccessObject.Models;

public partial class DoctorAndMedFacilitySearchContext : DbContext
{
    public DoctorAndMedFacilitySearchContext()
    {
    }

    public DoctorAndMedFacilitySearchContext(DbContextOptions<DoctorAndMedFacilitySearchContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Appointment> Appointments { get; set; }

    public virtual DbSet<DoctorProfile> DoctorProfiles { get; set; }

    public virtual DbSet<MedicalFacility> MedicalFacilities { get; set; }

    public virtual DbSet<PatientProfile> PatientProfiles { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<VwAppointment> VwAppointments { get; set; }

    public virtual DbSet<VwDoctorProfile> VwDoctorProfiles { get; set; }

    public virtual DbSet<VwMedicalFacility> VwMedicalFacilities { get; set; }

    public virtual DbSet<VwPatientProfile> VwPatientProfiles { get; set; }

    public virtual DbSet<VwReview> VwReviews { get; set; }

    public virtual DbSet<VwRole> VwRoles { get; set; }

    public virtual DbSet<VwUser> VwUsers { get; set; }

    private string GetConnectionString()
    {
        IConfiguration config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json",true,true)
            .Build();
        var strConn = config["ConnectionStrings:DefaultConnection"];

        return strConn;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer(GetConnectionString());
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.AppointmentId).HasName("PK__Appointm__8ECDFCA2BB21C066");

            entity.Property(e => e.AppointmentId).HasColumnName("AppointmentID");
            entity.Property(e => e.AppointmentDate).HasColumnType("datetime");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.DoctorId).HasColumnName("DoctorID");
            entity.Property(e => e.FacilityId).HasColumnName("FacilityID");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PatientId).HasColumnName("PatientID");
            entity.Property(e => e.PaymentStatus).HasMaxLength(50);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);

            entity.HasOne(d => d.Doctor).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.DoctorId)
                .HasConstraintName("FK__Appointme__Docto__59FA5E80");

            entity.HasOne(d => d.Facility).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.FacilityId)
                .HasConstraintName("FK__Appointme__Facil__5AEE82B9");

            entity.HasOne(d => d.Patient).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.PatientId)
                .HasConstraintName("FK__Appointme__Patie__59063A47");
        });

        modelBuilder.Entity<DoctorProfile>(entity =>
        {
            entity.HasKey(e => e.DoctorId).HasName("PK__DoctorPr__2DC00EDFDC2FC436");

            entity.Property(e => e.DoctorId)
                .ValueGeneratedNever()
                .HasColumnName("DoctorID");
            entity.Property(e => e.Availability).HasMaxLength(255);
            entity.Property(e => e.ConsultationFee).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Qualification).HasMaxLength(255);
            entity.Property(e => e.Specialty).HasMaxLength(255);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.WorkSchedule).HasMaxLength(255);

            entity.HasOne(d => d.Doctor).WithOne(p => p.DoctorProfile)
                .HasForeignKey<DoctorProfile>(d => d.DoctorId)
                .HasConstraintName("FK__DoctorPro__Docto__3F466844");
        });

        modelBuilder.Entity<MedicalFacility>(entity =>
        {
            entity.HasKey(e => e.FacilityId).HasName("PK__MedicalF__5FB08B94E0B9F716");

            entity.HasIndex(e => e.Email, "UQ__MedicalF__A9D10534C53289B7").IsUnique();

            entity.Property(e => e.FacilityId).HasColumnName("FacilityID");
            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.DoctorId).HasColumnName("DoctorID");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.OpeningHours).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(50);
            entity.Property(e => e.Rating).HasDefaultValue(0.0);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);

            entity.HasOne(d => d.Doctor).WithMany(p => p.MedicalFacilities)
                .HasForeignKey(d => d.DoctorId)
                .HasConstraintName("FK__MedicalFa__Docto__45F365D3");
        });

        modelBuilder.Entity<PatientProfile>(entity =>
        {
            entity.HasKey(e => e.PatientId).HasName("PK__PatientP__970EC346C0D6E5FD");

            entity.Property(e => e.PatientId)
                .ValueGeneratedNever()
                .HasColumnName("PatientID");
            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.BloodType).HasMaxLength(5);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.EmergencyContact).HasMaxLength(255);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);

            entity.HasOne(d => d.Patient).WithOne(p => p.PatientProfile)
                .HasForeignKey<PatientProfile>(d => d.PatientId)
                .HasConstraintName("FK__PatientPr__Patie__398D8EEE");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.ReviewId).HasName("PK__Reviews__74BC79AE310F54FD");

            entity.Property(e => e.ReviewId).HasColumnName("ReviewID");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.DoctorId).HasColumnName("DoctorID");
            entity.Property(e => e.FacilityId).HasColumnName("FacilityID");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PatientId).HasColumnName("PatientID");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);

            entity.HasOne(d => d.Doctor).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.DoctorId)
                .HasConstraintName("FK__Reviews__DoctorI__6C190EBB");

            entity.HasOne(d => d.Facility).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.FacilityId)
                .HasConstraintName("FK__Reviews__Facilit__6D0D32F4");

            entity.HasOne(d => d.Patient).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reviews__Patient__6B24EA82");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE3A2FF1EA75");

            entity.HasIndex(e => e.RoleName, "UQ__Roles__8A2B6160653E07B0").IsUnique();

            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.RoleName).HasMaxLength(50);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCACCE7D31A6");

            entity.HasIndex(e => e.PhoneNumber, "UQ__Users__85FB4E38765E2A1B").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534E4AF3310").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.FullName).HasMaxLength(255);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.PhoneNumber).HasMaxLength(50);
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Users__RoleID__34C8D9D1");
        });

        modelBuilder.Entity<VwAppointment>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("VW_Appointments");

            entity.Property(e => e.AppointmentDate).HasColumnType("datetime");
            entity.Property(e => e.AppointmentId)
                .ValueGeneratedOnAdd()
                .HasColumnName("AppointmentID");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.DoctorId).HasColumnName("DoctorID");
            entity.Property(e => e.FacilityId).HasColumnName("FacilityID");
            entity.Property(e => e.PatientId).HasColumnName("PatientID");
            entity.Property(e => e.PaymentStatus).HasMaxLength(50);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
        });

        modelBuilder.Entity<VwDoctorProfile>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("VW_DoctorProfiles");

            entity.Property(e => e.Availability).HasMaxLength(255);
            entity.Property(e => e.ConsultationFee).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.DoctorId).HasColumnName("DoctorID");
            entity.Property(e => e.Qualification).HasMaxLength(255);
            entity.Property(e => e.Specialty).HasMaxLength(255);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.WorkSchedule).HasMaxLength(255);
        });

        modelBuilder.Entity<VwMedicalFacility>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("VW_MedicalFacilities");

            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.DoctorId).HasColumnName("DoctorID");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.FacilityId)
                .ValueGeneratedOnAdd()
                .HasColumnName("FacilityID");
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.OpeningHours).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(50);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
        });

        modelBuilder.Entity<VwPatientProfile>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("VW_PatientProfiles");

            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.BloodType).HasMaxLength(5);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.EmergencyContact).HasMaxLength(255);
            entity.Property(e => e.PatientId).HasColumnName("PatientID");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
        });

        modelBuilder.Entity<VwReview>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("VW_Reviews");

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.DoctorId).HasColumnName("DoctorID");
            entity.Property(e => e.FacilityId).HasColumnName("FacilityID");
            entity.Property(e => e.PatientId).HasColumnName("PatientID");
            entity.Property(e => e.ReviewId)
                .ValueGeneratedOnAdd()
                .HasColumnName("ReviewID");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
        });

        modelBuilder.Entity<VwRole>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("VW_Roles");

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.RoleId)
                .ValueGeneratedOnAdd()
                .HasColumnName("RoleID");
            entity.Property(e => e.RoleName).HasMaxLength(50);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
        });

        modelBuilder.Entity<VwUser>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("VW_Users");

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.FullName).HasMaxLength(255);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.PhoneNumber).HasMaxLength(50);
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
            entity.Property(e => e.UserId)
                .ValueGeneratedOnAdd()
                .HasColumnName("UserID");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
