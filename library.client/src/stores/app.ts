import { defineStore } from 'pinia'
import { reactive, ref } from 'vue'

export const useAppStore = defineStore('app', () => {
  const sidebar = reactive({
    opened: true,
    withoutAnimation: false
  })

  const device = ref<'desktop' | 'mobile'>('desktop')

  function toggleSidebar() {
    sidebar.opened = !sidebar.opened
    sidebar.withoutAnimation = false
  }

  function closeSidebar(options: { withoutAnimation: boolean }) {
    sidebar.withoutAnimation = options.withoutAnimation
    sidebar.opened = false
  }

  function openSidebar(options: { withoutAnimation: boolean }) {
    sidebar.withoutAnimation = options.withoutAnimation
    sidebar.opened = true
  }

  function toggleDevice(val: 'desktop' | 'mobile') {
    device.value = val
  }

  return {
    sidebar,
    device,
    toggleSidebar,
    closeSidebar,
    openSidebar,
    toggleDevice
  }
})
