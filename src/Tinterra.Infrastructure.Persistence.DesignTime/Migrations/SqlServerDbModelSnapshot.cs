using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Tinterra.Infrastructure.Persistence.SqlServer;

#nullable disable

namespace Tinterra.Infrastructure.Persistence.DesignTime.Migrations;

[DbContext(typeof(SqlServerDb))]
partial class SqlServerDbModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasAnnotation("ProductVersion", "9.0.0-preview.6.24327.7");

        modelBuilder.Entity("Tinterra.Domain.Entities.AllowedTenant", b =>
        {
            b.Property<Guid>("TenantId")
                .HasColumnType("uniqueidentifier");

            b.Property<DateTime>("CreatedAtUtc")
                .HasColumnType("datetime2");

            b.Property<string>("DisplayName")
                .IsRequired()
                .HasMaxLength(200)
                .HasColumnType("nvarchar(200)");

            b.Property<bool>("IsEnabled")
                .HasColumnType("bit");

            b.HasKey("TenantId");

            b.HasIndex("DisplayName");

            b.ToTable("AllowedTenants");
        });

        modelBuilder.Entity("Tinterra.Domain.Entities.BundlePermission", b =>
        {
            b.Property<string>("BundleName")
                .HasMaxLength(200)
                .HasColumnType("nvarchar(200)");

            b.Property<string>("PermissionName")
                .HasMaxLength(200)
                .HasColumnType("nvarchar(200)");

            b.HasKey("BundleName", "PermissionName");

            b.HasIndex("PermissionName");

            b.ToTable("BundlePermissions");
        });

        modelBuilder.Entity("Tinterra.Domain.Entities.ConfigurationItem", b =>
        {
            b.Property<Guid>("Id")
                .HasColumnType("uniqueidentifier");

            b.Property<string>("Classification")
                .IsRequired()
                .HasMaxLength(20)
                .HasColumnType("nvarchar(20)");

            b.Property<string>("Environment")
                .IsRequired()
                .HasMaxLength(20)
                .HasColumnType("nvarchar(20)");

            b.Property<string>("Key")
                .IsRequired()
                .HasMaxLength(200)
                .HasColumnType("nvarchar(200)");

            b.Property<string>("OwnerObjectId")
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnType("nvarchar(100)");

            b.Property<string>("Region")
                .IsRequired()
                .HasMaxLength(10)
                .HasColumnType("nvarchar(10)");

            b.Property<string>("Status")
                .IsRequired()
                .HasMaxLength(20)
                .HasColumnType("nvarchar(20)");

            b.Property<DateTime>("UpdatedAtUtc")
                .HasColumnType("datetime2");

            b.Property<string>("Value")
                .IsRequired()
                .HasMaxLength(4000)
                .HasColumnType("nvarchar(4000)");

            b.Property<int>("Version")
                .HasColumnType("int");

            b.HasKey("Id");

            b.HasIndex("Key", "Environment", "Region")
                .IsUnique();

            b.ToTable("ConfigurationItems");
        });

        modelBuilder.Entity("Tinterra.Domain.Entities.Permission", b =>
        {
            b.Property<string>("Name")
                .HasMaxLength(200)
                .HasColumnType("nvarchar(200)");

            b.Property<string>("Description")
                .IsRequired()
                .HasMaxLength(500)
                .HasColumnType("nvarchar(500)");

            b.Property<bool>("IsSystem")
                .HasColumnType("bit");

            b.HasKey("Name");

            b.ToTable("Permissions");
        });

        modelBuilder.Entity("Tinterra.Domain.Entities.PermissionBundle", b =>
        {
            b.Property<string>("Name")
                .HasMaxLength(200)
                .HasColumnType("nvarchar(200)");

            b.Property<string>("Description")
                .IsRequired()
                .HasMaxLength(500)
                .HasColumnType("nvarchar(500)");

            b.Property<bool>("IsSystem")
                .HasColumnType("bit");

            b.HasKey("Name");

            b.ToTable("PermissionBundles");
        });

        modelBuilder.Entity("Tinterra.Domain.Entities.TenantGroupBundleMapping", b =>
        {
            b.Property<Guid>("Id")
                .HasColumnType("uniqueidentifier");

            b.Property<string>("BundleName")
                .IsRequired()
                .HasMaxLength(200)
                .HasColumnType("nvarchar(200)");

            b.Property<string>("GroupObjectId")
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnType("nvarchar(100)");

            b.Property<Guid>("TenantId")
                .HasColumnType("uniqueidentifier");

            b.HasKey("Id");

            b.HasIndex("TenantId", "GroupObjectId");

            b.ToTable("TenantGroupBundleMappings");
        });

        modelBuilder.Entity("Tinterra.Domain.Entities.UserAllowedRegion", b =>
        {
            b.Property<Guid>("Id")
                .HasColumnType("uniqueidentifier");

            b.Property<string>("Region")
                .IsRequired()
                .HasMaxLength(10)
                .HasColumnType("nvarchar(10)");

            b.Property<string>("UserObjectId")
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnType("nvarchar(100)");

            b.HasKey("Id");

            b.HasIndex("UserObjectId", "Region")
                .IsUnique();

            b.ToTable("UserAllowedRegions");
        });

        modelBuilder.Entity("Tinterra.Domain.Entities.UserProfile", b =>
        {
            b.Property<string>("ObjectId")
                .HasMaxLength(100)
                .HasColumnType("nvarchar(100)");

            b.Property<int>("ApprovalLevel")
                .HasColumnType("int");

            b.HasKey("ObjectId");

            b.ToTable("UserProfiles");
        });

        modelBuilder.Entity("Tinterra.Domain.Entities.UserAllowedRegion", b =>
        {
            b.HasOne("Tinterra.Domain.Entities.UserProfile", null)
                .WithMany("AllowedRegions")
                .HasForeignKey("UserObjectId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
        });
    }
}
