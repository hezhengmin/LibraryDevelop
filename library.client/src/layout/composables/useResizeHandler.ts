import { watch, onMounted, onBeforeUnmount } from 'vue'
import { useRoute } from 'vue-router'
import { useAppStore } from '@/stores/app'

const WIDTH = 768 // mobile breakpoint

export function useResizeHandler() {
  const appStore = useAppStore()
  const route = useRoute()

  function isMobile() {
    const rect = document.body.getBoundingClientRect()
    return rect.width - 1 < WIDTH
  }

  function resizeHandler() {
    if (!document.hidden) {
      const mobile = isMobile()
      appStore.toggleDevice(mobile ? 'mobile' : 'desktop')

      if (mobile) {
        appStore.closeSidebar({ withoutAnimation: true })
      }
    }
  }

  onMounted(() => {
    const mobile = isMobile()
    if (mobile) {
      appStore.toggleDevice('mobile')
      appStore.closeSidebar({ withoutAnimation: true })
    }
    window.addEventListener('resize', resizeHandler)
  })

  onBeforeUnmount(() => {
    window.removeEventListener('resize', resizeHandler)
  })

  // auto close sidebar on mobile route change
  watch(
    () => route.path,
    () => {
      if (appStore.device === 'mobile' && appStore.sidebar.opened) {
        appStore.closeSidebar({ withoutAnimation: false })
      }
    }
  )
}
