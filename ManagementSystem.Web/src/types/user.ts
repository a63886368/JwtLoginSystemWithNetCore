export interface UserDto {
  id: number
  userName: string
  email: string
  phone?: string
  isActive: boolean
  createTime: string
  roles: string[]
}

export interface CreateUserDto {
  userName: string
  password: string
  email: string
  phone?: string
  roleIds: number[]
}

export interface UpdateUserDto {
  email?: string
  phone?: string
  isActive?: boolean
  roleIds?: number[]
}

