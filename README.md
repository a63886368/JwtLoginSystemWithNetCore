# ManagementSystem - 管理系统框架

基于 .NET Core 6.0 + Vue 3 的分层管理系统框架

## 项目结构

```
ManagementSystem/
├── ManagementSystem.API/          # Web API 项目
├── ManagementSystem.Application/  # 应用层
├── ManagementSystem.Domain/       # 领域层
├── ManagementSystem.Infrastructure/ # 基础设施层
├── ManagementSystem.Web/          # 前端项目 (Vue 3)
└── ManagementSystem.sln           # 解决方案文件
```

## 技术栈

### 后端
- .NET Core 6.0
- Entity Framework Core 6.0
- SQL Server
- JWT 认证
- BCrypt 密码加密

### 前端
- Vue 3
- TypeScript
- Vite
- Element Plus
- Pinia
- Vue Router
- Axios

## 功能特性

- ✅ 用户登录认证（JWT）
- ✅ 用户管理（增删改查）
- ✅ 角色管理（增删改查）
- ✅ 动态菜单管理
- ✅ 基于角色的权限控制
- ✅ 分层架构设计

## 快速开始

### 1. 配置数据库

修改 `ManagementSystem.API/appsettings.json` 中的数据库连接字符串：

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=ManagementSystem;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

### 2. 运行后端

```bash
cd ManagementSystem.API
dotnet restore
dotnet run
```

API 将在 `http://localhost:5000` 启动，Swagger 文档在 `http://localhost:5000/swagger`

### 3. 运行前端

```bash
cd ManagementSystem.Web
npm install
npm run dev
```

前端将在 `http://localhost:5173` 启动

### 4. 初始化数据

首次运行时会自动创建数据库并初始化以下数据：

**默认管理员账户：**
- 用户名：`admin`
- 密码：`admin123`
- 角色：`Admin`

**默认角色：**
- `Admin` - 管理员角色（拥有所有权限）
- `User` - 普通用户角色

**默认菜单：**
- 首页 (`/dashboard`)
- 用户管理 (`/users`)
- 角色管理 (`/roles`)

## API 端点

### 认证
- `POST /api/auth/login` - 用户登录

### 用户管理（需要 Admin 角色）
- `GET /api/users` - 获取所有用户
- `GET /api/users/{id}` - 获取用户详情
- `POST /api/users` - 创建用户
- `PUT /api/users/{id}` - 更新用户
- `DELETE /api/users/{id}` - 删除用户

### 角色管理（需要 Admin 角色）
- `GET /api/roles` - 获取所有角色
- `GET /api/roles/{id}` - 获取角色详情
- `POST /api/roles` - 创建角色
- `PUT /api/roles/{id}` - 更新角色
- `DELETE /api/roles/{id}` - 删除角色

### 菜单管理
- `GET /api/menus/my-menus` - 获取当前用户的菜单
- `GET /api/menus` - 获取所有菜单（Admin）
- `POST /api/menus` - 创建菜单（Admin）
- `PUT /api/menus/{id}` - 更新菜单（Admin）
- `DELETE /api/menus/{id}` - 删除菜单（Admin）

## JWT 配置

在 `appsettings.json` 中配置 JWT 参数：

```json
{
  "JwtSettings": {
    "SecretKey": "YourSuperSecretKeyThatShouldBeAtLeast32CharactersLong!",
    "Issuer": "ManagementSystem",
    "Audience": "ManagementSystem",
    "ExpirationMinutes": "60"
  }
}
```

## 开发说明

### 数据库迁移

项目使用 `EnsureCreated` 自动创建数据库。如需使用迁移：

```bash
cd ManagementSystem.Infrastructure
dotnet ef migrations add InitialCreate --startup-project ../ManagementSystem.API
dotnet ef database update --startup-project ../ManagementSystem.API
```

### 添加新功能

1. 在 `Domain` 层定义实体和接口
2. 在 `Infrastructure` 层实现仓储和数据库上下文
3. 在 `Application` 层创建 DTO 和服务
4. 在 `API` 层创建控制器
5. 在前端添加相应的页面和 API 调用

## 注意事项

1. 首次运行前请确保 SQL Server 已安装并运行
2. 修改 JWT SecretKey 为更安全的密钥
3. 生产环境请使用 HTTPS
4. 建议使用环境变量存储敏感配置

## 许可证

MIT

