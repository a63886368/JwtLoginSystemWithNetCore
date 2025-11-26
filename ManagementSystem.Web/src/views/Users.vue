<template>
  <div class="users-container">
    <el-card>
      <template #header>
        <div class="card-header">
          <span>用户管理</span>
          <el-button type="primary" @click="handleAdd">添加用户</el-button>
        </div>
      </template>
      <el-table :data="users" style="width: 100%">
        <el-table-column prop="id" label="ID" width="80" />
        <el-table-column prop="userName" label="用户名" />
        <el-table-column prop="email" label="邮箱" />
        <el-table-column prop="phone" label="电话" />
        <el-table-column prop="isActive" label="状态">
          <template #default="{ row }">
            <el-tag :type="row.isActive ? 'success' : 'danger'">
              {{ row.isActive ? '启用' : '禁用' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="roles" label="角色">
          <template #default="{ row }">
            <el-tag v-for="role in row.roles" :key="role" style="margin-right: 5px">
              {{ role }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="200">
          <template #default="{ row }">
            <el-button size="small" @click="handleEdit(row)">编辑</el-button>
            <el-button size="small" type="danger" @click="handleDelete(row.id)">删除</el-button>
          </template>
        </el-table-column>
      </el-table>
    </el-card>

    <el-dialog v-model="dialogVisible" :title="dialogTitle" width="500px">
      <el-form :model="form" :rules="rules" ref="formRef" label-width="100px">
        <el-form-item label="用户名" prop="userName" v-if="!isEdit">
          <el-input v-model="form.userName" />
        </el-form-item>
        <el-form-item label="密码" prop="password" v-if="!isEdit">
          <el-input v-model="form.password" type="password" />
        </el-form-item>
        <el-form-item label="邮箱" prop="email">
          <el-input v-model="form.email" />
        </el-form-item>
        <el-form-item label="电话" prop="phone">
          <el-input v-model="form.phone" />
        </el-form-item>
        <el-form-item label="角色" prop="roleIds">
          <el-select v-model="form.roleIds" multiple placeholder="请选择角色" style="width: 100%">
            <el-option
              v-for="role in allRoles"
              :key="role.id"
              :label="role.roleName"
              :value="role.id"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="状态" prop="isActive" v-if="isEdit">
          <el-switch v-model="form.isActive" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button type="primary" @click="handleSubmit">确定</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import type { FormInstance, FormRules } from 'element-plus'
import { getUsers, createUser, updateUser, deleteUser } from '@/api/user'
import { getRoles } from '@/api/role'
import type { UserDto, CreateUserDto, UpdateUserDto } from '@/types/user'
import type { RoleDto } from '@/types/role'

const users = ref<UserDto[]>([])
const allRoles = ref<RoleDto[]>([])
const dialogVisible = ref(false)
const dialogTitle = ref('添加用户')
const isEdit = ref(false)
const formRef = ref<FormInstance>()
const currentId = ref<number | null>(null)

const form = reactive<CreateUserDto & UpdateUserDto & { isActive?: boolean }>({
  userName: '',
  password: '',
  email: '',
  phone: '',
  roleIds: [],
  isActive: true
})

const rules = reactive<FormRules>({
  userName: [{ required: true, message: '请输入用户名', trigger: 'blur' }],
  password: [{ required: true, message: '请输入密码', trigger: 'blur' }],
  email: [
    { required: true, message: '请输入邮箱', trigger: 'blur' },
    { type: 'email', message: '请输入正确的邮箱格式', trigger: 'blur' }
  ]
})

const loadUsers = async () => {
  try {
    users.value = await getUsers()
  } catch (error) {
    ElMessage.error('加载用户列表失败')
  }
}

const loadRoles = async () => {
  try {
    allRoles.value = await getRoles()
  } catch (error) {
    ElMessage.error('加载角色列表失败')
  }
}

const handleAdd = () => {
  isEdit.value = false
  dialogTitle.value = '添加用户'
  Object.assign(form, {
    userName: '',
    password: '',
    email: '',
    phone: '',
    roleIds: [],
    isActive: true
  })
  dialogVisible.value = true
}

const handleEdit = (row: UserDto) => {
  isEdit.value = true
  dialogTitle.value = '编辑用户'
  currentId.value = row.id
  Object.assign(form, {
    email: row.email,
    phone: row.phone || '',
    roleIds: [],
    isActive: row.isActive
  })
  dialogVisible.value = true
}

const handleSubmit = async () => {
  if (!formRef.value) return
  
  await formRef.value.validate(async (valid) => {
    if (valid) {
      try {
        if (isEdit.value && currentId.value) {
          const updateDto: UpdateUserDto = {
            email: form.email,
            phone: form.phone,
            isActive: form.isActive,
            roleIds: form.roleIds
          }
          await updateUser(currentId.value, updateDto)
          ElMessage.success('更新成功')
        } else {
          const createDto: CreateUserDto = {
            userName: form.userName,
            password: form.password,
            email: form.email,
            phone: form.phone,
            roleIds: form.roleIds || []
          }
          await createUser(createDto)
          ElMessage.success('创建成功')
        }
        dialogVisible.value = false
        loadUsers()
      } catch (error: any) {
        ElMessage.error(error.response?.data?.message || '操作失败')
      }
    }
  })
}

const handleDelete = async (id: number) => {
  try {
    await ElMessageBox.confirm('确定要删除该用户吗？', '提示', {
      type: 'warning'
    })
    await deleteUser(id)
    ElMessage.success('删除成功')
    loadUsers()
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('删除失败')
    }
  }
}

onMounted(() => {
  loadUsers()
  loadRoles()
})
</script>

<style scoped>
.users-container {
  padding: 20px;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}
</style>

