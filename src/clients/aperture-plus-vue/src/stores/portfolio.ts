// src/stores/portfolio.ts
import { defineStore } from 'pinia'
import { ref } from 'vue'
import type { Portfolio, Gallery } from '@/types/portfolio'
import apiClient from '@/api/axios'
import { ElMessage, ElMessageBox } from 'element-plus'
import type { ParamsSerializerOptions } from 'axios'

export const usePortfolioStore = defineStore('portfolio', () => {
  // --- State ---
  // 这个 state 现在是 "当前正在查看的" 作品集
  const portfolio = ref<Portfolio | null>(null)
  const isLoading = ref(false)

  const baseUrl = import.meta.env.VITE_API_PORTFOLIO_BASE_URL

  // --- 【统一】 Action 1：获取作品集 ---
  // 此函数现在是唯一的入口
  async function fetchPortfolio(userId: string | null = null) {
    isLoading.value = true
    // 每次 fetch 时先清空旧数据，防止B用户页面显示A用户数据
    portfolio.value = null

    try {
      let response
      if (userId) {
        // 1. 获取 "他人" 的作品集
        response = await apiClient.get<Portfolio>(
          `${baseUrl}/portfolios/GetPortfolioByUserId/${userId}`,
        )
      } else {
        // 2. 获取 "我的" 作品集
        response = await apiClient.get<Portfolio>(`${baseUrl}/portfolios/GetMyPortfolio`)
      }
      portfolio.value = response.data
    } catch (error) {
      console.error('获取作品集失败:', error)
      // ElMessage.error('获取作品集数据失败') // 暂时关闭，因为 404 (没作品集) 也会触发
      // 确保即使失败，portfolio 也是一个有效（但为空）的对象结构
      portfolio.value = {
        portfolioId: '',
        userId: userId || '',
        galleries: [],
      }
    } finally {
      isLoading.value = false
    }
  }

  // --- 其他 Action 保持不变 ---
  // (createGallery, deleteGallery, uploadPhotos, deletePhotos...)
  // 它们只应在 isOwner=true 的页面被调用，所以它们会正确地操作 "我的" 作品集
  // (后端 API 也是受 [Authorize] 保护的)

  async function createGallery(galleryName: string) {
    if (!galleryName.trim()) {
      ElMessage.warning('相册名称不能为空')
      return false
    }
    try {
      const response = await apiClient.post<string>(
        `${baseUrl}/portfolios/CreateGallery/${galleryName}`,
      )
      const newGalleryId = response.data
      if (portfolio.value) {
        const newGallery: Gallery = {
          galleryId: newGalleryId,
          galleryName,
          photos: [],
          createdAt: new Date().toISOString(),
          coverPhotoUrl: undefined,
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

  async function deleteGallery(galleryId: string) {
    try {
      await ElMessageBox.confirm(
        '确定要删除这个相册吗？相册内的所有照片都将被删除，此操作不可撤销。',
        '确认删除',
        {
          confirmButtonText: '确定',
          cancelButtonText: '取消',
          type: 'warning',
        },
      )
      await apiClient.delete(`${baseUrl}/portfolios/DeleteGallery/${galleryId}`)
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

  async function uploadPhotos(galleryId: string, files: File[]) {
    // ... (此函数保持不变) ...
    if (files.length === 0) return false
    const formData = new FormData()
    files.forEach((file) => {
      formData.append('files', file)
    })
    try {
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
      // 【重要】上传后，我们强制刷新 "我的" 作品集
      await fetchPortfolio(null)
      return true
    } catch (error) {
      console.error('上传照片失败:', error)
      ElMessage.error('上传照片失败')
      return false
    }
  }

  async function deletePhotos(galleryId: string, photoIds: string[]) {
    // ... (此函数保持不变) ...
    if (photoIds.length === 0) return
    try {
      await ElMessageBox.confirm(`确定要删除选中的 ${photoIds.length} 张照片吗？`, '确认删除', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning',
      })

      await apiClient.delete(`${baseUrl}/portfolios/DeletePhoto/${galleryId}`, {
        params: {
          photoIds: photoIds,
        },
        paramsSerializer: {
          serialize: (params) => {
            const searchParams = new URLSearchParams()
            for (const key in params) {
              const value = params[key]
              if (Array.isArray(value)) {
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

  function clearPortfolio() {
    portfolio.value = null
  }

  return {
    portfolio,
    isLoading,
    // 【统一】只导出一个 fetchPortfolio
    fetchPortfolio,
    createGallery,
    deleteGallery,
    uploadPhotos,
    deletePhotos,
    clearPortfolio,
  }
})
