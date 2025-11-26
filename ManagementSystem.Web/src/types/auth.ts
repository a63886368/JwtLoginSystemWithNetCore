export interface LoginDto {
  userName: string
  password: string
}

export interface LoginResponseDto {
  token: string
  user: UserDto
  roles: string[]
}

export interface UserDto {
  id: number
  userName: string
  email: string
  phone?: string
  isActive: boolean
  createTime: string
  roles: string[]
}

