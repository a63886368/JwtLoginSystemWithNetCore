import api from './axios'
import type { UserDto, CreateUserDto, UpdateUserDto } from '@/types/user'

export const getUsers = (): Promise<UserDto[]> => {
  return api.get('/users')
}

export const getUserById = (id: number): Promise<UserDto> => {
  return api.get(`/users/${id}`)
}

export const createUser = (user: CreateUserDto): Promise<UserDto> => {
  return api.post('/users', user)
}

export const updateUser = (id: number, user: UpdateUserDto): Promise<UserDto> => {
  return api.put(`/users/${id}`, user)
}

export const deleteUser = (id: number): Promise<void> => {
  return api.delete(`/users/${id}`)
}

