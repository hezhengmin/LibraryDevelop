<template>
  <div class="empty-view">
    <el-empty :description="description">
      <template #image>
        <el-icon :size="100" color="#909399">
          <DocumentCopy />
        </el-icon>
      </template>
    </el-empty>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useRoute } from 'vue-router'
import { DocumentCopy } from '@element-plus/icons-vue'

const route = useRoute()

const description = computed(() => {
  const title = route.meta?.title as string
  return title ? `${title} 頁面` : '此頁面尚未設定內容'
})
</script>

<style scoped lang="scss">
.empty-view {
  display: flex;
  align-items: center;
  justify-content: center;
  min-height: 400px;
  padding: 40px;
}
</style>
