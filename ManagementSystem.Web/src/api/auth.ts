import api from './axios'
import type { LoginDto, LoginResponseDto } from '@/types/auth'

export const login = (loginDto: LoginDto): Promise<LoginResponseDto> => {
  return api.post('/auth/login', loginDto)
}

