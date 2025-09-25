// src/stores/portfolio.ts

import { defineStore } from 'pinia'
import { ref } from 'vue'
import type { Portfolio, Gallery } from '@/types/portfolio'
import apiClient from '@/api/axios'
import { ElMessage, ElMessageBox } from 'element-plus'
import type { ParamsSerializerOptions } from 'axios'

export const usePortfolioStore = defineStore('portfolio', () => {
  // --- State ---
  const portfolio = ref<Portfolio | null>(null)
  const isLoading = ref(false)

  // --- Actions ---

  // --- 新增：删除相册 ---
  async function deleteGallery(galleryId: string) {
    try {
      // 弹出确认框
      await ElMessageBox.confirm(
        '确定要删除这个相册吗？相册内的所有照片都将被删除，此操作不可撤销。',
        '确认删除',
        {
          confirmButtonText: '确定',
          cancelButtonText: '取消',
          type: 'warning',
        },
      )

      const baseUrl = import.meta.env.VITE_API_PORTFOLIO_BASE_URL
      await apiClient.delete(`${baseUrl}/portfolios/DeleteGallery/${galleryId}`)

      // 成功后，乐观更新本地数据
      if (portfolio.value) {
        portfolio.value.galleries = portfolio.value.galleries.filter(
          (g) => g.galleryId !== galleryId,
        )
      }
      ElMessage.success('相册已删除！')
    } catch (error) {
      if (error !== 'cancel') {
        console.error('删除相册失败:', error)
        ElMessage.error('删除相册失败')
      } else {
        ElMessage.info('已取消删除')
      }
    }
  }

  // --- 新增：上传照片 ---
  async function uploadPhotos(galleryId: string, files: File[]) {
    if (files.length === 0) return

    const formData = new FormData()
    files.forEach((file) => {
      formData.append('files', file)
    })

    try {
      const baseUrl = import.meta.env.VITE_API_PORTFOLIO_BASE_URL
      // 后端接口返回了新照片的ID列表
      await apiClient.post<{ photoIds: string[] }>(
        `${baseUrl}/portfolios/UploadPhotos/${galleryId}`,
        formData,
        {
          headers: {
            'Content-Type': 'multipart/form-data',
          },
        },
      )

      ElMessage.success(`${files.length} 张照片上传成功！`)

      // 上传成功后，重新获取整个作品集来更新UI，这是最简单可靠的方式
      await fetchPortfolio(true) // 传入true强制刷新

      return true
    } catch (error) {
      console.error('上传照片失败:', error)
      ElMessage.error('上传照片失败')
      return false
    }
  }

  // --- 新增：删除照片 ---
  async function deletePhotos(galleryId: string, photoIds: string[]) {
    if (photoIds.length === 0) return
    try {
      await ElMessageBox.confirm(`确定要删除选中的 ${photoIds.length} 张照片吗？`, '确认删除', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning',
      })

      const baseUrl = import.meta.env.VITE_API_PORTFOLIO_BASE_URL
      // 后端需要一个特定的格式来接收数组，我们使用 params 发送
      await apiClient.delete(`${baseUrl}/portfolios/DeletePhoto/${galleryId}`, {
        params: { photoIds: photoIds },
        paramsSerializer: {
          serialize: (params) => {
            // 使用 URLSearchParams 来构建查询字符串
            const searchParams = new URLSearchParams()
            for (const key in params) {
              const value = params[key]
              if (Array.isArray(value)) {
                // 如果是数组，为每个元素重复添加键
                value.forEach((item) => {
                  searchParams.append(key, item)
                })
              } else {
                searchParams.append(key, value)
              }
            }
            return searchParams.toString()
          },
        } as ParamsSerializerOptions,
      })

      // 成功后，乐观更新本地数据
      if (portfolio.value) {
        const gallery = portfolio.value.galleries.find((g) => g.galleryId === galleryId)
        if (gallery) {
          gallery.photos = gallery.photos.filter((p) => !photoIds.includes(p.photoId))
        }
      }
      ElMessage.success('照片已删除！')
    } catch (error) {
      if (error !== 'cancel') {
        console.error('删除照片失败:', error)
        ElMessage.error('删除照片失败')
      } else {
        ElMessage.info('已取消删除')
      }
    }
  }

  // 强制刷新作品集
  async function fetchPortfolio(force = false) {
    if (portfolio.value && !force) return
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
    deleteGallery,
    uploadPhotos,
    deletePhotos,
    clearPortfolio,
  }
})
