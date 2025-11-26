-- =============================================
-- ManagementSystem 数据库初始化脚本
-- SQL Server 2019
-- =============================================

USE master;
GO

-- 创建数据库（如果不存在）
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'ManagementSystem')
BEGIN
    CREATE DATABASE ManagementSystem;
END
GO

USE ManagementSystem;
GO

-- =============================================
-- 删除现有表（如果存在，按依赖关系顺序）
-- =============================================
IF OBJECT_ID('RoleMenus', 'U') IS NOT NULL DROP TABLE RoleMenus;
IF OBJECT_ID('UserRoles', 'U') IS NOT NULL DROP TABLE UserRoles;
IF OBJECT_ID('Menus', 'U') IS NOT NULL DROP TABLE Menus;
IF OBJECT_ID('Users', 'U') IS NOT NULL DROP TABLE Users;
IF OBJECT_ID('Roles', 'U') IS NOT NULL DROP TABLE Roles;
GO

-- =============================================
-- 创建表结构
-- =============================================

-- 创建 Roles 表
CREATE TABLE Roles (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    RoleName NVARCHAR(50) NOT NULL UNIQUE,
    Description NVARCHAR(500) NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreateTime DATETIME NOT NULL DEFAULT GETDATE(),
    UpdateTime DATETIME NULL
);
GO

-- 创建 Users 表
CREATE TABLE Users (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserName NVARCHAR(50) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    Phone NVARCHAR(20) NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreateTime DATETIME NOT NULL DEFAULT GETDATE(),
    UpdateTime DATETIME NULL
);
GO

-- 创建 Menus 表
CREATE TABLE Menus (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    MenuName NVARCHAR(50) NOT NULL,
    MenuCode NVARCHAR(50) NULL,
    Path NVARCHAR(200) NULL,
    Icon NVARCHAR(50) NULL,
    ParentId INT NULL,
    SortOrder INT NOT NULL DEFAULT 0,
    IsVisible BIT NOT NULL DEFAULT 1,
    CreateTime DATETIME NOT NULL DEFAULT GETDATE(),
    UpdateTime DATETIME NULL,
    CONSTRAINT FK_Menus_Parent FOREIGN KEY (ParentId) REFERENCES Menus(Id) ON DELETE NO ACTION
);
GO

-- 创建 UserRoles 表（用户角色关联表）
CREATE TABLE UserRoles (
    UserId INT NOT NULL,
    RoleId INT NOT NULL,
    PRIMARY KEY (UserId, RoleId),
    CONSTRAINT FK_UserRoles_User FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
    CONSTRAINT FK_UserRoles_Role FOREIGN KEY (RoleId) REFERENCES Roles(Id) ON DELETE CASCADE
);
GO

-- 创建 RoleMenus 表（角色菜单关联表）
CREATE TABLE RoleMenus (
    RoleId INT NOT NULL,
    MenuId INT NOT NULL,
    PRIMARY KEY (RoleId, MenuId),
    CONSTRAINT FK_RoleMenus_Role FOREIGN KEY (RoleId) REFERENCES Roles(Id) ON DELETE CASCADE,
    CONSTRAINT FK_RoleMenus_Menu FOREIGN KEY (MenuId) REFERENCES Menus(Id) ON DELETE CASCADE
);
GO

-- =============================================
-- 创建索引
-- =============================================
CREATE INDEX IX_Users_UserName ON Users(UserName);
CREATE INDEX IX_Users_Email ON Users(Email);
CREATE INDEX IX_Roles_RoleName ON Roles(RoleName);
CREATE INDEX IX_Menus_ParentId ON Menus(ParentId);
GO

-- =============================================
-- 插入初始数据
-- =============================================

-- 插入角色数据
SET IDENTITY_INSERT Roles ON;
INSERT INTO Roles (Id, RoleName, Description, IsActive, CreateTime) VALUES
(1, N'Admin', N'管理员角色', 1, GETDATE()),
(2, N'User', N'普通用户角色', 1, GETDATE());
SET IDENTITY_INSERT Roles OFF;
GO

-- 插入用户数据
-- 密码: admin123 (BCrypt 哈希值)
-- 注意：BCrypt 每次生成的哈希值都不同，但都能验证通过
-- 如果此哈希值无法验证，请运行以下 C# 代码生成新的哈希值：
-- BCrypt.Net.BCrypt.HashPassword("admin123")
-- 然后将生成的哈希值替换下面的值
SET IDENTITY_INSERT Users ON;
INSERT INTO Users (Id, UserName, PasswordHash, Email, IsActive, CreateTime) VALUES
(1, N'admin', N'$2a$11$YawLeXkewEhiHhkJ1DcQeuPIV0/f.ykcxDNM2efljyGML91ROOQ0K', N'admin@example.com', 1, GETDATE());
SET IDENTITY_INSERT Users OFF;
GO

-- 插入菜单数据
SET IDENTITY_INSERT Menus ON;
INSERT INTO Menus (Id, MenuName, MenuCode, Path, Icon, ParentId, SortOrder, IsVisible, CreateTime) VALUES
(1, N'首页', N'dashboard', N'/dashboard', N'HomeFilled', NULL, 1, 1, GETDATE()),
(2, N'用户管理', N'users', N'/users', N'User', NULL, 2, 1, GETDATE()),
(3, N'角色管理', N'roles', N'/roles', N'UserFilled', NULL, 3, 1, GETDATE());
SET IDENTITY_INSERT Menus OFF;
GO

-- 插入用户角色关联数据
INSERT INTO UserRoles (UserId, RoleId) VALUES
(1, 1);  -- admin 用户拥有 Admin 角色
GO

-- 插入角色菜单关联数据（Admin 角色拥有所有菜单权限）
INSERT INTO RoleMenus (RoleId, MenuId) VALUES
(1, 1),  -- Admin 角色拥有首页菜单
(1, 2),  -- Admin 角色拥有用户管理菜单
(1, 3);  -- Admin 角色拥有角色管理菜单
GO

-- =============================================
-- 验证数据
-- =============================================
SELECT 'Roles' AS TableName, COUNT(*) AS RecordCount FROM Roles
UNION ALL
SELECT 'Users', COUNT(*) FROM Users
UNION ALL
SELECT 'Menus', COUNT(*) FROM Menus
UNION ALL
SELECT 'UserRoles', COUNT(*) FROM UserRoles
UNION ALL
SELECT 'RoleMenus', COUNT(*) FROM RoleMenus;
GO

PRINT '数据库初始化完成！';
PRINT '默认管理员账号: admin';
PRINT '默认密码: admin123';
GO

