<template>
  <div class="home min-h-screen bg-gradient-to-r from-blue-500 to-purple-600 flex items-center justify-center">
    <div class="bg-white rounded-lg shadow-xl p-8 max-w-md">
      <h1 class="text-4xl font-bold text-gray-800 mb-4">Home Page</h1>
      <p class="text-gray-600 mb-6">Welcome to your Vue 3 application with Tailwind CSS!</p>
      <el-button type="primary" @click="goToAbout" class="w-full">
        Go to About
      </el-button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useRouter } from 'vue-router'

const router = useRouter()

const goToAbout = () => {
  router.push('/about')
}
</script>

