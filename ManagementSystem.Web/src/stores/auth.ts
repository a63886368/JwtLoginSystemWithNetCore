import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { login } from '@/api/auth'
import type { LoginDto, LoginResponseDto } from '@/types/auth'

export const useAuthStore = defineStore('auth', () => {
  const token = ref<string | null>(localStorage.getItem('token'))
  const user = ref<LoginResponseDto['user'] | null>(null)
  const roles = ref<string[]>([])

  const isAuthenticated = computed(() => !!token.value)

  async function handleLogin(loginDto: LoginDto) {
    try {
      const response = await login(loginDto)
      token.value = response.token
      user.value = response.user
      roles.value = response.roles
      localStorage.setItem('token', response.token)
      return true
    } catch (error) {
      console.error('登录失败:', error)
      return false
    }
  }

  function logout() {
    token.value = null
    user.value = null
    roles.value = []
    localStorage.removeItem('token')
  }

  return {
    token,
    user,
    roles,
    isAuthenticated,
    handleLogin,
    logout
  }
})

