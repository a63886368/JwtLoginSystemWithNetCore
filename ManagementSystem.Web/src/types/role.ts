export interface RoleDto {
  id: number
  roleName: string
  description?: string
  isActive: boolean
  createTime: string
}

export interface CreateRoleDto {
  roleName: string
  description?: string
  menuIds?: number[]
}

