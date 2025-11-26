import api from './axios'
import type { RoleDto, CreateRoleDto } from '@/types/role'

export const getRoles = (): Promise<RoleDto[]> => {
  return api.get('/roles')
}

export const getRoleById = (id: number): Promise<RoleDto> => {
  return api.get(`/roles/${id}`)
}

export const createRole = (role: CreateRoleDto): Promise<RoleDto> => {
  return api.post('/roles', role)
}

export const updateRole = (id: number, role: CreateRoleDto): Promise<RoleDto> => {
  return api.put(`/roles/${id}`, role)
}

export const deleteRole = (id: number): Promise<void> => {
  return api.delete(`/roles/${id}`)
}

