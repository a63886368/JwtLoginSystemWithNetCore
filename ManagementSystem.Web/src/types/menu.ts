export interface MenuDto {
  id: number
  menuName: string
  menuCode?: string
  path?: string
  icon?: string
  parentId?: number
  sortOrder: number
  isVisible: boolean
  children?: MenuDto[]
}

