<template>
  <div class="roles-container">
    <el-card>
      <template #header>
        <div class="card-header">
          <span>角色管理</span>
          <el-button type="primary" @click="handleAdd">添加角色</el-button>
        </div>
      </template>
      <el-table :data="roles" style="width: 100%">
        <el-table-column prop="id" label="ID" width="80" />
        <el-table-column prop="roleName" label="角色名" />
        <el-table-column prop="description" label="描述" />
        <el-table-column prop="isActive" label="状态">
          <template #default="{ row }">
            <el-tag :type="row.isActive ? 'success' : 'danger'">
              {{ row.isActive ? '启用' : '禁用' }}
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
        <el-form-item label="角色名" prop="roleName">
          <el-input v-model="form.roleName" />
        </el-form-item>
        <el-form-item label="描述" prop="description">
          <el-input v-model="form.description" type="textarea" />
        </el-form-item>
        <el-form-item label="菜单权限" prop="menuIds">
          <el-tree
            ref="treeRef"
            :data="menuTree"
            :props="{ children: 'children', label: 'menuName' }"
            show-checkbox
            node-key="id"
            :default-checked-keys="form.menuIds"
          />
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
import { getRoles, createRole, updateRole, deleteRole } from '@/api/role'
import { getMenus } from '@/api/menu'
import type { RoleDto, CreateRoleDto } from '@/types/role'
import type { MenuDto } from '@/types/menu'

const roles = ref<RoleDto[]>([])
const menuTree = ref<MenuDto[]>([])
const dialogVisible = ref(false)
const dialogTitle = ref('添加角色')
const formRef = ref<FormInstance>()
const treeRef = ref()
const currentId = ref<number | null>(null)

const form = reactive<CreateRoleDto>({
  roleName: '',
  description: '',
  menuIds: []
})

const rules = reactive<FormRules>({
  roleName: [{ required: true, message: '请输入角色名', trigger: 'blur' }]
})

const loadRoles = async () => {
  try {
    roles.value = await getRoles()
  } catch (error) {
    ElMessage.error('加载角色列表失败')
  }
}

const loadMenus = async () => {
  try {
    menuTree.value = await getMenus()
  } catch (error) {
    ElMessage.error('加载菜单列表失败')
  }
}

const handleAdd = () => {
  dialogTitle.value = '添加角色'
  currentId.value = null
  Object.assign(form, {
    roleName: '',
    description: '',
    menuIds: []
  })
  dialogVisible.value = true
  setTimeout(() => {
    if (treeRef.value) {
      treeRef.value.setCheckedKeys([])
    }
  }, 100)
}

const handleEdit = (row: RoleDto) => {
  dialogTitle.value = '编辑角色'
  currentId.value = row.id
  Object.assign(form, {
    roleName: row.roleName,
    description: row.description || '',
    menuIds: []
  })
  dialogVisible.value = true
}

const handleSubmit = async () => {
  if (!formRef.value) return
  
  await formRef.value.validate(async (valid) => {
    if (valid) {
      try {
        const checkedKeys = treeRef.value?.getCheckedKeys() || []
        form.menuIds = checkedKeys
        
        if (currentId.value) {
          await updateRole(currentId.value, form)
          ElMessage.success('更新成功')
        } else {
          await createRole(form)
          ElMessage.success('创建成功')
        }
        dialogVisible.value = false
        loadRoles()
      } catch (error: any) {
        ElMessage.error(error.response?.data?.message || '操作失败')
      }
    }
  })
}

const handleDelete = async (id: number) => {
  try {
    await ElMessageBox.confirm('确定要删除该角色吗？', '提示', {
      type: 'warning'
    })
    await deleteRole(id)
    ElMessage.success('删除成功')
    loadRoles()
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('删除失败')
    }
  }
}

onMounted(() => {
  loadRoles()
  loadMenus()
})
</script>

<style scoped>
.roles-container {
  padding: 20px;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}
</style>

