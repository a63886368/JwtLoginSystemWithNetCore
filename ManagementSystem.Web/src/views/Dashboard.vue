<template>
  <div class="dashboard">
    <h1>欢迎使用管理系统</h1>
    <el-row :gutter="20" style="margin-top: 20px">
      <el-col :span="8">
        <el-card>
          <template #header>
            <span>用户统计</span>
          </template>
          <div class="stat-value">{{ userCount }}</div>
        </el-card>
      </el-col>
      <el-col :span="8">
        <el-card>
          <template #header>
            <span>角色统计</span>
          </template>
          <div class="stat-value">{{ roleCount }}</div>
        </el-card>
      </el-col>
      <el-col :span="8">
        <el-card>
          <template #header>
            <span>菜单统计</span>
          </template>
          <div class="stat-value">{{ menuCount }}</div>
        </el-card>
      </el-col>
    </el-row>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { getUsers } from '@/api/user'
import { getRoles } from '@/api/role'
import { getMenus } from '@/api/menu'

const userCount = ref(0)
const roleCount = ref(0)
const menuCount = ref(0)

onMounted(async () => {
  try {
    const [users, roles, menus] = await Promise.all([
      getUsers(),
      getRoles(),
      getMenus()
    ])
    userCount.value = users.length
    roleCount.value = roles.length
    menuCount.value = menus.length
  } catch (error) {
    console.error('加载统计数据失败:', error)
  }
})
</script>

<style scoped>
.dashboard {
  padding: 20px;
}

.stat-value {
  font-size: 32px;
  font-weight: bold;
  text-align: center;
  color: #409eff;
}
</style>

