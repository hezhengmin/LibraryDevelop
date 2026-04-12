<script setup lang="ts">
import type { SysMenu } from "@/api/sysMenu";

const props = defineProps<{
  item: SysMenu;
}>();
</script>

<template>
  <!-- 無子選單的選單項 -->
  <el-menu-item
    v-if="!item.children || item.children.length === 0"
    :index="item.url || '/'"
  >
    <el-icon v-if="item.icon">
      <component :is="item.icon" />
    </el-icon>
    <template #title>{{ item.name }}</template>
  </el-menu-item>

  <!-- 有子選單的選單項 -->
  <el-sub-menu v-else :index="String(item.id)">
    <template #title>
      <el-icon v-if="item.icon">
        <component :is="item.icon" />
      </el-icon>
      <span>{{ item.name }}</span>
    </template>
    <SidebarItem
      v-for="child in item.children"
      :key="child.id"
      :item="child"
    />
  </el-sub-menu>
</template>
