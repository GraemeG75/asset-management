-- ============================================================================
-- DATABASE-CREATE.SQL
-- T-SQL Database & Application User Creation Script
-- Database Name: AssetManagementDb
-- App User: AssetMgmtAppUser (Read and Write Privileges Only)
-- Rules:
-- 1. Sets up database container and options for metadata and domain tables
-- 2. Creates server login and database user with db_datareader & db_datawriter privileges
-- 3. ALL metadata tables/views use 'x_' and 'vw_x_' prefixes
-- 4. ALL primary/foreign keys use UNIQUEIDENTIFIER
-- 5. ALL string columns use NVARCHAR
-- ============================================================================

-- 1. Create Database Container
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = N'AssetManagementDb')
BEGIN
    CREATE DATABASE [AssetManagementDb]
    COLLATE Latin1_General_100_CI_AS_SC_UTF8;
    
    ALTER DATABASE [AssetManagementDb] SET READ_COMMITTED_SNAPSHOT ON;
    ALTER DATABASE [AssetManagementDb] SET ALLOW_SNAPSHOT_ISOLATION ON;
    
    PRINT N'Database AssetManagementDb created successfully.';
END
ELSE
BEGIN
    PRINT N'Database AssetManagementDb already exists.';
END
GO

USE [AssetManagementDb];
GO

-- 2. Create Server-Level Login for Application (Read/Write Privileges Only)
IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = N'AssetMgmtAppUser')
BEGIN
    CREATE LOGIN [AssetMgmtAppUser] 
    WITH PASSWORD = N'StrongP@ssw0rd!2026', 
         CHECK_POLICY = OFF, 
         CHECK_EXPIRATION = OFF;
    PRINT N'Server Login AssetMgmtAppUser created successfully.';
END
GO

-- 3. Create Database-Level User & Grant Read / Write Privileges
IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = N'AssetMgmtAppUser')
BEGIN
    CREATE USER [AssetMgmtAppUser] FOR LOGIN [AssetMgmtAppUser];
    PRINT N'Database User AssetMgmtAppUser created successfully.';
END
GO

-- Assign Read and Write Roles
ALTER ROLE [db_datareader] ADD MEMBER [AssetMgmtAppUser];
ALTER ROLE [db_datawriter] ADD MEMBER [AssetMgmtAppUser];

-- Grant Stored Procedure Execution Rights
GRANT EXECUTE TO [AssetMgmtAppUser];

PRINT N'Granted db_datareader, db_datawriter, and EXECUTE privileges to AssetMgmtAppUser.';
GO
