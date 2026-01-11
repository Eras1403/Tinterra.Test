using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tinterra.Infrastructure.Persistence.DesignTime.Migrations;

[DbContext(typeof(Tinterra.Infrastructure.Persistence.SqlServer.SqlServerDb))]
[Migration("20240914120000_InitialCreate")]
public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "AllowedTenants",
            columns: table => new
            {
                TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                DisplayName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AllowedTenants", x => x.TenantId);
            });

        migrationBuilder.CreateTable(
            name: "ConfigurationItems",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Key = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                Value = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                Environment = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                Region = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                OwnerObjectId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                Classification = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                Version = table.Column<int>(type: "int", nullable: false),
                UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ConfigurationItems", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "PermissionBundles",
            columns: table => new
            {
                Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                IsSystem = table.Column<bool>(type: "bit", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_PermissionBundles", x => x.Name);
            });

        migrationBuilder.CreateTable(
            name: "Permissions",
            columns: table => new
            {
                Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                IsSystem = table.Column<bool>(type: "bit", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Permissions", x => x.Name);
            });

        migrationBuilder.CreateTable(
            name: "TenantGroupBundleMappings",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                GroupObjectId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                BundleName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_TenantGroupBundleMappings", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "UserProfiles",
            columns: table => new
            {
                ObjectId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                ApprovalLevel = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_UserProfiles", x => x.ObjectId);
            });

        migrationBuilder.CreateTable(
            name: "BundlePermissions",
            columns: table => new
            {
                BundleName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                PermissionName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_BundlePermissions", x => new { x.BundleName, x.PermissionName });
            });

        migrationBuilder.CreateTable(
            name: "UserAllowedRegions",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                UserObjectId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                Region = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_UserAllowedRegions", x => x.Id);
                table.ForeignKey(
                    name: "FK_UserAllowedRegions_UserProfiles_UserObjectId",
                    column: x => x.UserObjectId,
                    principalTable: "UserProfiles",
                    principalColumn: "ObjectId",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_AllowedTenants_DisplayName",
            table: "AllowedTenants",
            column: "DisplayName");

        migrationBuilder.CreateIndex(
            name: "IX_BundlePermissions_PermissionName",
            table: "BundlePermissions",
            column: "PermissionName");

        migrationBuilder.CreateIndex(
            name: "IX_ConfigurationItems_Key_Environment_Region",
            table: "ConfigurationItems",
            columns: new[] { "Key", "Environment", "Region" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_TenantGroupBundleMappings_TenantId_GroupObjectId",
            table: "TenantGroupBundleMappings",
            columns: new[] { "TenantId", "GroupObjectId" });

        migrationBuilder.CreateIndex(
            name: "IX_UserAllowedRegions_UserObjectId_Region",
            table: "UserAllowedRegions",
            columns: new[] { "UserObjectId", "Region" },
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "AllowedTenants");
        migrationBuilder.DropTable(name: "BundlePermissions");
        migrationBuilder.DropTable(name: "ConfigurationItems");
        migrationBuilder.DropTable(name: "PermissionBundles");
        migrationBuilder.DropTable(name: "Permissions");
        migrationBuilder.DropTable(name: "TenantGroupBundleMappings");
        migrationBuilder.DropTable(name: "UserAllowedRegions");
        migrationBuilder.DropTable(name: "UserProfiles");
    }
}
