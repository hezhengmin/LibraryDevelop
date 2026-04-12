<script setup lang="ts">
import { computed, onMounted } from "vue";
import { useRouter } from "vue-router";
import { useAuthStore } from "@/stores/auth";
import { useMenuStore } from "@/stores/menu";
import { useAppStore } from "@/stores/app";
import { useResizeHandler } from "./composables/useResizeHandler";
import Sidebar from "./components/Sidebar/index.vue";
import Navbar from "./components/Navbar.vue";

const router = useRouter();
const authStore = useAuthStore();
const menuStore = useMenuStore();
const appStore = useAppStore();

useResizeHandler();

const sidebar = computed(() => appStore.sidebar);
const device = computed(() => appStore.device);

const classObj = computed(() => ({
  hideSidebar: !sidebar.value.opened,
  openSidebar: sidebar.value.opened,
  withoutAnimation: sidebar.value.withoutAnimation,
  mobile: device.value === "mobile",
}));

const handleClickOutside = () => {
  appStore.closeSidebar({ withoutAnimation: false });
};

// 組件掛載時載入選單（如果還沒載入）
onMounted(async () => {
  if (!menuStore.routesLoaded) {
    await menuStore.loadNavigationMenus();
  }
});
</script>

<template>
  <div :class="classObj" class="app-wrapper">
    <div
      v-if="device === 'mobile' && sidebar.opened"
      class="drawer-bg"
      @click="handleClickOutside"
    />
    <Sidebar class="sidebar-container" />
    <div class="main-container">
      <div class="fixed-header">
        <Navbar />
      </div>
      <main class="app-main">
        <router-view />
      </main>
    </div>
  </div>
</template>

<style scoped lang="scss">

.app-wrapper {
  @include clearfix;
  position: relative;
  height: 100%;
  width: 100%;

  &.mobile.openSidebar {
    position: fixed;
    top: 0;
  }
}

.drawer-bg {
  background: #000;
  opacity: 0.3;
  width: 100%;
  top: 0;
  height: 100%;
  position: absolute;
  z-index: 999;
}

.fixed-header {
  position: fixed;
  top: 0;
  right: 0;
  z-index: 9;
  width: calc(100% - $sidebar-width);
  transition: width 0.28s;
}

.hideSidebar .fixed-header {
  width: calc(100% - $sidebar-collapsed-width);
}

.mobile .fixed-header {
  width: 100%;
}

.app-main {
  min-height: calc(100vh - $header-height);
  width: 100%;
  position: relative;
  overflow: hidden;
  padding: 20px;
  margin-top: $header-height;
}
</style>
