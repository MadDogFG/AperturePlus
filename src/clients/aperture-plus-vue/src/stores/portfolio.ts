// src/stores/portfolio.ts

import { defineStore } from 'pinia'
import { ref } from 'vue'
import type { Portfolio, Gallery } from '@/types/portfolio'
import apiClient from '@/api/axios'
import { ElMessage } from 'element-plus'

export const usePortfolioStore = defineStore('portfolio', () => {
  // --- State ---
  const portfolio = ref<Portfolio | null>(null)
  const isLoading = ref(false)

  // --- Actions ---

  // 获取用户的完整作品集
  async function fetchPortfolio() {
    if (portfolio.value) return // 防止重复获取
    isLoading.value = true
    try {
      const baseUrl = import.meta.env.VITE_API_PORTFOLIO_BASE_URL
      const response = await apiClient.get<Portfolio>(`${baseUrl}/portfolios/GetPortfolioByUserId`)
      portfolio.value = response.data
    } catch (error) {
      console.error('获取作品集失败:', error)
      ElMessage.error('获取作品集数据失败')
    } finally {
      isLoading.value = false
    }
  }

  // 创建新的相册
  async function createGallery(galleryName: string) {
    if (!galleryName.trim()) {
      ElMessage.warning('相册名称不能为空')
      return
    }
    try {
      const baseUrl = import.meta.env.VITE_API_PORTFOLIO_BASE_URL
      const response = await apiClient.post<string>(
        `${baseUrl}/portfolios/CreateGallery/${galleryName}`,
      )
      const newGalleryId = response.data

      // 成功后，乐观更新本地数据，避免重新请求
      if (portfolio.value) {
        const newGallery: Gallery = {
          galleryId: newGalleryId,
          galleryName,
          photos: [],
          createdAt: new Date().toISOString(),
        }
        portfolio.value.galleries.push(newGallery)
      }
      ElMessage.success('相册创建成功！')
      return true
    } catch (error) {
      console.error('创建相册失败:', error)
      ElMessage.error('创建相册失败')
      return false
    }
  }

  // 清空作品集数据（例如，用户退出登录时）
  function clearPortfolio() {
    portfolio.value = null
  }

  return {
    portfolio,
    isLoading,
    fetchPortfolio,
    createGallery,
    clearPortfolio,
  }
})
