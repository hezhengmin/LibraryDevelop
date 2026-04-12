<script setup lang="ts">
import { computed } from "vue";
import { useRoute } from "vue-router";
import { useMenuStore } from "@/stores/menu";
import { useAppStore } from "@/stores/app";
import SidebarItem from "./SidebarItem.vue";

const route = useRoute();
const menuStore = useMenuStore();
const appStore = useAppStore();

const menuItems = computed(() => menuStore.navigationMenus);
const loading = computed(() => !menuStore.routesLoaded);
const isCollapse = computed(() => !appStore.sidebar.opened);
const activeMenu = computed(() => {
  const { meta, path } = route;
  if (meta.activeMenu) return meta.activeMenu as string;
  return path;
});
</script>

<template>
  <div>
    <el-scrollbar wrap-class="scrollbar-wrapper">
      <el-menu
        :default-active="activeMenu"
        :collapse="isCollapse"
        :background-color="'#304156'"
        :text-color="'#bfcbd9'"
        :unique-opened="false"
        :active-text-color="'#409eff'"
        :collapse-transition="false"
        mode="vertical"
        router
        v-loading="loading"
      >
        <SidebarItem
          v-for="item in menuItems"
          :key="item.id"
          :item="item"
        />
      </el-menu>
    </el-scrollbar>
  </div>
</template>
