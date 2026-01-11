SET NOCOUNT ON;

DECLARE @TenantId UNIQUEIDENTIFIER = '11111111-1111-1111-1111-111111111111';
DECLARE @GroupId UNIQUEIDENTIFIER = '22222222-2222-2222-2222-222222222222';

MERGE INTO AllowedTenants AS target
USING (SELECT @TenantId AS TenantId, 'Contoso AG' AS DisplayName) AS source
ON target.TenantId = source.TenantId
WHEN MATCHED THEN
    UPDATE SET DisplayName = source.DisplayName, IsEnabled = 1
WHEN NOT MATCHED THEN
    INSERT (TenantId, DisplayName, IsEnabled, CreatedAtUtc)
    VALUES (source.TenantId, source.DisplayName, 1, SYSUTCDATETIME());

MERGE INTO Permissions AS target
USING (VALUES
    ('Config.View', 'View configuration items', 1),
    ('Config.Edit', 'Edit configuration items', 1),
    ('Config.Publish', 'Publish configuration items', 1),
    ('Config.Delete', 'Delete configuration items', 1),
    ('Security.Admin', 'Security administration', 1)
) AS source(Name, Description, IsSystem)
ON target.Name = source.Name
WHEN MATCHED THEN
    UPDATE SET Description = source.Description, IsSystem = source.IsSystem
WHEN NOT MATCHED THEN
    INSERT (Name, Description, IsSystem)
    VALUES (source.Name, source.Description, source.IsSystem);

MERGE INTO PermissionBundles AS target
USING (VALUES
    ('Config.Reader', 'Read-only configuration access', 1),
    ('Config.Editor', 'Edit configuration access', 1),
    ('Config.Publisher', 'Publish configuration access', 1),
    ('Config.Admin', 'Full configuration access', 1),
    ('Security.Admin', 'Security administration', 1)
) AS source(Name, Description, IsSystem)
ON target.Name = source.Name
WHEN MATCHED THEN
    UPDATE SET Description = source.Description, IsSystem = source.IsSystem
WHEN NOT MATCHED THEN
    INSERT (Name, Description, IsSystem)
    VALUES (source.Name, source.Description, source.IsSystem);

MERGE INTO BundlePermissions AS target
USING (VALUES
    ('Config.Reader', 'Config.View'),
    ('Config.Editor', 'Config.View'),
    ('Config.Editor', 'Config.Edit'),
    ('Config.Publisher', 'Config.View'),
    ('Config.Publisher', 'Config.Edit'),
    ('Config.Publisher', 'Config.Publish'),
    ('Config.Admin', 'Config.View'),
    ('Config.Admin', 'Config.Edit'),
    ('Config.Admin', 'Config.Publish'),
    ('Config.Admin', 'Config.Delete'),
    ('Security.Admin', 'Security.Admin')
) AS source(BundleName, PermissionName)
ON target.BundleName = source.BundleName AND target.PermissionName = source.PermissionName
WHEN NOT MATCHED THEN
    INSERT (BundleName, PermissionName)
    VALUES (source.BundleName, source.PermissionName);

MERGE INTO TenantGroupBundleMappings AS target
USING (SELECT NEWID() AS Id, @TenantId AS TenantId, CAST(@GroupId AS NVARCHAR(100)) AS GroupObjectId, 'Security.Admin' AS BundleName) AS source
ON target.TenantId = source.TenantId AND target.GroupObjectId = source.GroupObjectId AND target.BundleName = source.BundleName
WHEN NOT MATCHED THEN
    INSERT (Id, TenantId, GroupObjectId, BundleName)
    VALUES (source.Id, source.TenantId, source.GroupObjectId, source.BundleName);

MERGE INTO UserProfiles AS target
USING (SELECT '33333333-3333-3333-3333-333333333333' AS ObjectId, 2 AS ApprovalLevel) AS source
ON target.ObjectId = source.ObjectId
WHEN MATCHED THEN
    UPDATE SET ApprovalLevel = source.ApprovalLevel
WHEN NOT MATCHED THEN
    INSERT (ObjectId, ApprovalLevel)
    VALUES (source.ObjectId, source.ApprovalLevel);

MERGE INTO UserAllowedRegions AS target
USING (VALUES
    (NEWID(), '33333333-3333-3333-3333-333333333333', 'CH'),
    (NEWID(), '33333333-3333-3333-3333-333333333333', 'EU')
) AS source(Id, UserObjectId, Region)
ON target.UserObjectId = source.UserObjectId AND target.Region = source.Region
WHEN NOT MATCHED THEN
    INSERT (Id, UserObjectId, Region)
    VALUES (source.Id, source.UserObjectId, source.Region);

MERGE INTO ConfigurationItems AS target
USING (VALUES
    (NEWID(), 'ApiKey:Payments', 'secret-123', 'Prod', 'EU', '33333333-3333-3333-3333-333333333333', 'Confidential', 'Draft', 1, SYSUTCDATETIME()),
    (NEWID(), 'Feature:NewOnboarding', 'true', 'Test', 'CH', '33333333-3333-3333-3333-333333333333', 'Internal', 'Published', 2, SYSUTCDATETIME()),
    (NEWID(), 'Config:RegionBanner', 'Welcome', 'Dev', 'US', '33333333-3333-3333-3333-333333333333', 'Public', 'Draft', 1, SYSUTCDATETIME()),
    (NEWID(), 'Storage:Endpoint', 'https://storage', 'Prod', 'CH', '33333333-3333-3333-3333-333333333333', 'Internal', 'Published', 3, SYSUTCDATETIME()),
    (NEWID(), 'Feature:BetaUI', 'false', 'Dev', 'EU', '33333333-3333-3333-3333-333333333333', 'Public', 'Archived', 4, SYSUTCDATETIME())
) AS source(Id, [Key], [Value], [Environment], Region, OwnerObjectId, Classification, Status, Version, UpdatedAtUtc)
ON target.[Key] = source.[Key] AND target.[Environment] = source.[Environment] AND target.Region = source.Region
WHEN MATCHED THEN
    UPDATE SET [Value] = source.[Value], Classification = source.Classification, Status = source.Status, Version = source.Version, UpdatedAtUtc = source.UpdatedAtUtc
WHEN NOT MATCHED THEN
    INSERT (Id, [Key], [Value], [Environment], Region, OwnerObjectId, Classification, Status, Version, UpdatedAtUtc)
    VALUES (source.Id, source.[Key], source.[Value], source.[Environment], source.Region, source.OwnerObjectId, source.Classification, source.Status, source.Version, source.UpdatedAtUtc);
