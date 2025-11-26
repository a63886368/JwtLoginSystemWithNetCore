import api from './axios'
import type { MenuDto } from '@/types/menu'

export const getMyMenus = (): Promise<MenuDto[]> => {
  return api.get('/menus/my-menus')
}

export const getMenus = (): Promise<MenuDto[]> => {
  return api.get('/menus')
}

export const getMenuById = (id: number): Promise<MenuDto> => {
  return api.get(`/menus/${id}`)
}

export const createMenu = (menu: MenuDto): Promise<MenuDto> => {
  return api.post('/menus', menu)
}

export const updateMenu = (id: number, menu: MenuDto): Promise<MenuDto> => {
  return api.put(`/menus/${id}`, menu)
}

export const deleteMenu = (id: number): Promise<void> => {
  return api.delete(`/menus/${id}`)
}

