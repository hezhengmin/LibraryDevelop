<script setup lang="ts">
import { useRouter } from "vue-router";
import { ArrowDown } from "@element-plus/icons-vue";
import { useAuthStore } from "@/stores/auth";
import { useAppStore } from "@/stores/app";
import { useMenuStore } from "@/stores/menu";
import Breadcrumb from "./Breadcrumb.vue";
import Hamburger from "./Hamburger.vue";

const router = useRouter();
const authStore = useAuthStore();
const appStore = useAppStore();
const menuStore = useMenuStore();

const toggleSidebar = () => {
  appStore.toggleSidebar();
};

const logout = async () => {
  await authStore.logout();
  menuStore.clearMenus();
  router.push("/login");
};
</script>

<template>
  <div class="navbar">
    <Hamburger
      :is-active="appStore.sidebar.opened"
      class="hamburger-container"
      @toggle-click="toggleSidebar"
    />
    <Breadcrumb class="breadcrumb-container" />

    <div class="right-menu">
      <el-dropdown class="avatar-container" trigger="click">
        <div class="avatar-wrapper">
          <el-avatar :size="30" v-if="authStore.user">
            {{ authStore.user.username?.charAt(0).toUpperCase() }}
          </el-avatar>
          <span class="user-name" v-if="authStore.user">{{
            authStore.user.username
          }}</span>
          <el-icon class="el-icon--right"><arrow-down /></el-icon>
        </div>
        <template #dropdown>
          <el-dropdown-menu>
            <router-link to="/">
              <el-dropdown-item>首頁</el-dropdown-item>
            </router-link>
            <el-dropdown-item divided @click="logout">
              <span style="display: block">登出</span>
            </el-dropdown-item>
          </el-dropdown-menu>
        </template>
      </el-dropdown>
    </div>
  </div>
</template>

<style lang="scss" scoped>
.navbar {
  height: 50px;
  overflow: hidden;
  position: relative;
  background: #fff;
  box-shadow: 0 1px 4px rgba(0, 21, 41, 0.08);
  display: flex;
  align-items: center;

  .hamburger-container {
    line-height: 46px;
    height: 100%;
    float: left;
    cursor: pointer;
    transition: background 0.3s;
    -webkit-tap-highlight-color: transparent;
    padding: 0 15px;

    &:hover {
      background: rgba(0, 0, 0, 0.025);
    }
  }

  .breadcrumb-container {
    float: left;
  }

  .right-menu {
    margin-left: auto;
    height: 100%;
    line-height: 50px;
    display: flex;
    align-items: center;
    padding-right: 15px;

    &:focus {
      outline: none;
    }

    .avatar-container {
      cursor: pointer;

      .avatar-wrapper {
        display: flex;
        align-items: center;
        gap: 8px;
      }

      .user-name {
        font-size: 14px;
        color: #606266;
      }
    }
  }
}
</style>
