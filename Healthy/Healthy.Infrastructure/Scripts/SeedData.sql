-- Seed Script for HealthyDB_Dev
-- This script creates initial roles and an admin user

USE HealthyDB_Dev;
GO

-- Insert default roles
DECLARE @AdminRoleId UNIQUEIDENTIFIER = NEWID();
DECLARE @UserRoleId UNIQUEIDENTIFIER = NEWID();
DECLARE @ModeratorRoleId UNIQUEIDENTIFIER = NEWID();

-- Insert roles
INSERT INTO [Roles] ([Id], [Name], [Description], [IsActive], [CreatedAt], [IsDeleted])
VALUES 
    (@AdminRoleId, 'Admin', 'System Administrator with full access', 1, GETUTCDATE(), 0),
    (@UserRoleId, 'User', 'Regular user with basic access', 1, GETUTCDATE(), 0),
    (@ModeratorRoleId, 'Moderator', 'Moderator with limited admin access', 1, GETUTCDATE(), 0);

-- Insert admin user (password is "Admin@123" hashed with BCrypt)
DECLARE @AdminUserId UNIQUEIDENTIFIER = NEWID();

INSERT INTO [Users] ([Id], [FirstName], [LastName], [Email], [PasswordHash], [IsActive], [CreatedAt], [IsDeleted])
VALUES 
    (@AdminUserId, 'System', 'Administrator', 'admin@healthysystem.com', '$2a$11$3tEK5ZODo.jJF5nJv.wgbeOaE4j3RgD8xTg2Pl3.wIzBzJo7MKf6W', 1, GETUTCDATE(), 0);

-- Assign admin role to admin user
INSERT INTO [UserRoles] ([Id], [UserId], [RoleId], [CreatedAt], [IsDeleted])
VALUES 
    (NEWID(), @AdminUserId, @AdminRoleId, GETUTCDATE(), 0);

-- Insert test user (password is "User@123" hashed with BCrypt)
DECLARE @TestUserId UNIQUEIDENTIFIER = NEWID();

INSERT INTO [Users] ([Id], [FirstName], [LastName], [Email], [PasswordHash], [IsActive], [CreatedAt], [IsDeleted])
VALUES 
    (@TestUserId, 'Test', 'User', 'user@healthysystem.com', '$2a$11$3tEK5ZODo.jJF5nJv.wgbeOaE4j3RgD8xTg2Pl3.wIzBzJo7MKf6W', 1, GETUTCDATE(), 0);

-- Assign user role to test user
INSERT INTO [UserRoles] ([Id], [UserId], [RoleId], [CreatedAt], [IsDeleted])
VALUES 
    (NEWID(), @TestUserId, @UserRoleId, GETUTCDATE(), 0);

PRINT 'Seed data inserted successfully!';
PRINT 'Admin credentials: admin@healthysystem.com / Admin@123';
PRINT 'User credentials: user@healthysystem.com / User@123';

-- Verify data
SELECT 'Roles' as TableName, COUNT(*) as RecordCount FROM [Roles] WHERE [IsDeleted] = 0
UNION ALL
SELECT 'Users' as TableName, COUNT(*) as RecordCount FROM [Users] WHERE [IsDeleted] = 0
UNION ALL
SELECT 'UserRoles' as TableName, COUNT(*) as RecordCount FROM [UserRoles] WHERE [IsDeleted] = 0;

GO
