import { defineStore } from "pinia";
import { ref } from "vue";
import type { RouteRecordRaw } from "vue-router";
import sysMenuApi, { type SysMenu } from "@/api/sysMenu";
import router from "@/router";

/**
 * 選單路由 Store
 * 管理動態路由和導航選單
 */
export const useMenuStore = defineStore("menu", () => {
  // 導航選單數據
  const navigationMenus = ref<SysMenu[]>([]);

  // 是否已載入動態路由
  const routesLoaded = ref(false);

  /**
   * 使用 import.meta.glob 自動掃描 views 目錄下所有 .vue 組件
   * 後端 SysMenu.Component 欄位格式為 "/views/XxxView.vue"
   */
  const viewModules = import.meta.glob("@/views/**/*.vue") as Record<string, () => Promise<any>>;

  /**
   * 根據後端回傳的 Component 路徑取得組件
   */
  const getComponent = (componentPath: string | undefined): (() => Promise<any>) => {
    if (componentPath && componentPath.trim() !== "") {
      // 後端格式: "/views/XxxView.vue" → 轉為 glob key: "/src/views/XxxView.vue"
      const key = Object.keys(viewModules).find((k) =>
        k.endsWith(componentPath.replace(/^\//, "").replace(/^views\//, "views/"))
      );

      if (key) {
        return viewModules[key];
      }

      console.warn(`⚠️ 組件 ${componentPath} 在 views 目錄中找不到`);
    }

    // 回傳 EmptyView 作為 fallback
    const emptyKey = Object.keys(viewModules).find((k) => k.endsWith("EmptyView.vue"));
    return emptyKey ? viewModules[emptyKey] : () => import("@/views/EmptyView.vue");
  };

  /**
   * 根據選單數據生成路由配置
   */
  const generateRoutes = (menus: SysMenu[]): RouteRecordRaw[] => {
    const routes: RouteRecordRaw[] = [];

    const processMenu = (menu: SysMenu): RouteRecordRaw | null => {
      // 只處理啟用且可見的選單項
      if (!menu.status || menu.hidden) {
        return null;
      }

      // 如果有 URL 路徑，創建路由
      if (menu.url && menu.isMenu) {
        // 首頁已經在靜態路由中，跳過
        if (menu.url === "/") {
          return null;
        }

        // 移除開頭的 / 因為要作為 Layout 的 children
        let childPath = menu.url.startsWith("/")
          ? menu.url.substring(1)
          : menu.url;

        const route: RouteRecordRaw = {
          path: childPath, // 作為 children，不帶開頭的 /
          name: menu.moduleNo || `menu-${menu.id}`,
          component: getComponent(menu.component),
          meta: {
            requiresAuth: true,
            title: menu.name,
            icon: menu.icon,
            id: menu.id,
            moduleNo: menu.moduleNo,
          },
        };

        return route;
      }

      return null;
    };

    // 遞迴處理選單及其子選單
    const traverseMenus = (menuList: SysMenu[]) => {
      for (const menu of menuList) {
        const route = processMenu(menu);
        if (route) {
          routes.push(route);
        }

        // 處理子選單
        if (menu.children && menu.children.length > 0) {
          traverseMenus(menu.children);
        }
      }
    };

    traverseMenus(menus);
    return routes;
  };

  /**
   * 載入用戶導航選單並生成動態路由
   */
  const loadNavigationMenus = async () => {
    try {
      const response = await sysMenuApi.getUserNavigationMenus();

      if (response.succeeded && response.data) {
        navigationMenus.value = response.data;

        console.log("📋 選單數據:", response.data);

        // 生成動態路由
        const dynamicRoutes = generateRoutes(response.data);

        console.log("🛣️ 生成的路由:", dynamicRoutes);

        // 將動態路由添加到 Layout 路由的子路由中
        const layoutRoute = router.getRoutes().find((route) => route.path === "/");

        if (layoutRoute) {
          // 添加動態路由到 Layout 的 children
          dynamicRoutes.forEach((route) => {
            // 檢查路由是否已存在，避免重複添加
            const existingRoute = router.hasRoute(route.name as string);
            if (!existingRoute) {
              // 使用父路由的 name 來添加 children
              router.addRoute('Layout', route);
              console.log(`✅ 添加路由: ${route.path} -> ${String(route.name)}`);
            } else {
              console.log(`⚠️ 路由已存在: ${route.path}`);
            }
          });

          routesLoaded.value = true;
          console.log(`✅ 動態路由已載入: ${dynamicRoutes.length} 個路由`);
        }

        return true;
      } else {
        console.error("❌ 載入導航選單失敗:", response.message);
        return false;
      }
    } catch (error) {
      console.error("❌ 載入導航選單時發生錯誤:", error);
      return false;
    }
  };

  /**
   * 清除選單和路由狀態
   */
  const clearMenus = () => {
    navigationMenus.value = [];
    routesLoaded.value = false;
  };

  /**
   * 根據 moduleNo 查找選單項
   */
  const findMenuByModuleNo = (moduleNo: string): SysMenu | null => {
    const findInMenus = (menus: SysMenu[]): SysMenu | null => {
      for (const menu of menus) {
        if (menu.moduleNo === moduleNo) {
          return menu;
        }
        if (menu.children && menu.children.length > 0) {
          const found = findInMenus(menu.children);
          if (found) return found;
        }
      }
      return null;
    };

    return findInMenus(navigationMenus.value);
  };

  /**
   * 根據 URL 查找選單項
   */
  const findMenuByUrl = (url: string): SysMenu | null => {
    const findInMenus = (menus: SysMenu[]): SysMenu | null => {
      for (const menu of menus) {
        if (menu.url === url) {
          return menu;
        }
        if (menu.children && menu.children.length > 0) {
          const found = findInMenus(menu.children);
          if (found) return found;
        }
      }
      return null;
    };

    return findInMenus(navigationMenus.value);
  };

  return {
    navigationMenus,
    routesLoaded,
    loadNavigationMenus,
    clearMenus,
    findMenuByModuleNo,
    findMenuByUrl,
  };
});
