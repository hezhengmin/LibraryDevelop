<template>
  <div class="about min-h-screen bg-gradient-to-r from-green-400 to-blue-500 flex items-center justify-center">
    <div class="bg-white rounded-lg shadow-xl p-8 max-w-md">
      <h1 class="text-4xl font-bold text-gray-800 mb-4">About Page</h1>
      <p class="text-gray-600 mb-6">This is the about page with beautiful Tailwind CSS styling.</p>
      <el-button type="success" @click="goToHome" class="w-full">
        Go to Home
      </el-button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useRouter } from 'vue-router'

const router = useRouter()

const goToHome = () => {
  router.push('/')
}
</script>

