using System;
using System.Collections.Generic;
using System.Linq;
using ManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace ManagementSystem.Infrastructure.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            // 确保数据库已创建
            context.Database.EnsureCreated();

            // 检查是否已有数据
            if (context.Users.Any())
            {
                return; // 数据库已初始化
            }

            // 创建初始角色
            var adminRole = new Role
            {
                RoleName = "Admin",
                Description = "管理员角色",
                IsActive = true,
                CreateTime = DateTime.Now
            };

            var userRole = new Role
            {
                RoleName = "User",
                Description = "普通用户角色",
                IsActive = true,
                CreateTime = DateTime.Now
            };

            context.Roles.AddRange(adminRole, userRole);
            context.SaveChanges();

            // 创建初始管理员用户
            var adminUser = new User
            {
                UserName = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                Email = "admin@example.com",
                IsActive = true,
                CreateTime = DateTime.Now
            };

            context.Users.Add(adminUser);
            context.SaveChanges();

            // 分配管理员角色
            var adminUserRole = new UserRole
            {
                UserId = adminUser.Id,
                RoleId = adminRole.Id
            };

            context.UserRoles.Add(adminUserRole);

            // 创建初始菜单
            var menus = new List<Menu>
            {
                new Menu
                {
                    MenuName = "首页",
                    MenuCode = "dashboard",
                    Path = "/dashboard",
                    Icon = "HomeFilled",
                    SortOrder = 1,
                    IsVisible = true,
                    CreateTime = DateTime.Now
                },
                new Menu
                {
                    MenuName = "用户管理",
                    MenuCode = "users",
                    Path = "/users",
                    Icon = "User",
                    SortOrder = 2,
                    IsVisible = true,
                    CreateTime = DateTime.Now
                },
                new Menu
                {
                    MenuName = "角色管理",
                    MenuCode = "roles",
                    Path = "/roles",
                    Icon = "UserFilled",
                    SortOrder = 3,
                    IsVisible = true,
                    CreateTime = DateTime.Now
                }
            };

            context.Menus.AddRange(menus);
            context.SaveChanges();

            // 为管理员角色分配所有菜单权限
            foreach (var menu in menus)
            {
                context.RoleMenus.Add(new RoleMenu
                {
                    RoleId = adminRole.Id,
                    MenuId = menu.Id
                });
            }

            context.SaveChanges();
        }
    }
}

