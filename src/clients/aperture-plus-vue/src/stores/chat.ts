import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import * as signalR from '@microsoft/signalr'
import { useAuthStore } from './auth'
import apiClient from '@/api/axios'
import { useUserStore } from './user'
import { ElMessage } from 'element-plus'

// 1. 从 Controller 导入 DTOs
export interface ChatUserDto {
  userId: string
  userName: string
  avatarUrl: string
}
export interface Message {
  messageId: string
  senderId: string
  content: string
  timestamp: string
}
export interface ConversationListDto {
  conversationId: string
  participant: ChatUserDto
  lastMessage?: Message
  lastUpdatedAt: string
}
export interface ConversationDto {
  conversationId: string
  participants: ChatUserDto[]
  messages: Message[]
  lastUpdatedAt: string
}
export interface MessageDto extends Message {
  conversationId: string
  senderName: string
}

const chatApiBaseUrl = import.meta.env.VITE_API_CHAT_BASE_URL
const chatHubUrl = import.meta.env.VITE_API_CHAT_HUB_URL

let connectionPromise: Promise<void> | null = null

export const useChatStore = defineStore('chat', () => {
  const connection = ref<signalR.HubConnection | null>(null)

  // 状态
  const conversations = ref<ConversationListDto[]>([]) // 会话列表
  const activeConversation = ref<ConversationDto | null>(null) // 当前打开的会话
  const isChatPanelVisible = ref(false) // 聊天面板是否可见
  const isConversationsLoading = ref(false)
  const isMessagesLoading = ref(false)

  const authStore = useAuthStore()
  const userStore = useUserStore()

  // 辅助 getter
  const activeParticipant = computed(() => {
    if (!activeConversation.value || !authStore.profile) return null
    return (
      activeConversation.value.participants.find((p) => p.userId !== authStore.profile!.userId) ||
      null
    )
  })

  // 1. 启动 SignalR 连接
  async function startConnection() {
    if (!authStore.token) {
      return Promise.reject('No auth token')
    }

    // 如果连接已存在且已连接，直接返回
    if (connection.value && connection.value.state === signalR.HubConnectionState.Connected) {
      return Promise.resolve()
    }

    // 如果正在连接 (connectionPromise 存在)，返回现有的 promise
    if (connectionPromise) {
      return connectionPromise
    }

    // 创建新连接
    if (!connection.value) {
      connection.value = new signalR.HubConnectionBuilder()
        .withUrl(chatHubUrl, {
          accessTokenFactory: () => authStore.token!,
        })
        .withAutomaticReconnect()
        .build()

      // 注册监听
      connection.value.on('ReceiveMessage', (message: MessageDto) => {
        console.log('收到新消息:', message)
        const convoInList = conversations.value.find(
          (c) => c.conversationId === message.conversationId,
        )
        if (convoInList) {
          convoInList.lastMessage = message
          convoInList.lastUpdatedAt = message.timestamp
          conversations.value.sort(
            (a, b) => new Date(b.lastUpdatedAt).getTime() - new Date(a.lastUpdatedAt).getTime(),
          )
        }
        if (activeConversation.value?.conversationId === message.conversationId) {
          activeConversation.value.messages.push(message)
        }
      })
    }

    // 创建连接 Promise 锁
    connectionPromise = new Promise(async (resolve, reject) => {
      try {
        await connection.value!.start()
        console.log('SignalR (Chat) Connected.')
        await fetchConversations()
        resolve()
      } catch (err) {
        console.error('SignalR (Chat) Connection Error: ', err)
        reject(err)
      } finally {
        // 无论成功与否，都要释放锁
        connectionPromise = null
      }
    })

    return connectionPromise
  }

  // 2. (新) 拉取所有会话列表
  async function fetchConversations() {
    isConversationsLoading.value = true
    try {
      const response = await apiClient.get<ConversationListDto[]>(
        `${chatApiBaseUrl}/api/conversations`,
      )
      conversations.value = response.data
    } catch (e) {
      console.error('获取会话列表失败', e)
    } finally {
      isConversationsLoading.value = false
    }
  }

  // 3. (核心) 打开一个聊天窗口
  async function openChatWithUser(recipientId: string) {
    if (!userStore.profile || userStore.profile.userId === recipientId) {
      return
    }

    isChatPanelVisible.value = true
    activeConversation.value = null
    isMessagesLoading.value = true

    try {
      // 1. 【修改】确保 SignalR 优先连接并等待
      await startConnection()

      // 2. HTTP Call (获取/创建会话)
      const response = await apiClient.get<ConversationDto>(
        `${chatApiBaseUrl}/api/conversations/with/${recipientId}`,
      )
      activeConversation.value = response.data

      // 3. 【修改】现在我们可以安全地调用 invoke
      // 确保连接状态万无一失
      if (connection.value?.state === signalR.HubConnectionState.Connected) {
        // 这就是之前报错的 line 141
        await connection.value.invoke('JoinGroup', response.data.conversationId)
      } else {
        // 如果到这里，说明 startConnection 失败了
        throw new Error('SignalR connection failed to establish.')
      }

      // 4. 刷新会话列表
      await fetchConversations()
    } catch (e) {
      console.error('发起聊天失败:', e)
      // 抛出错误，让调用方 (Vue组件) 知道
      throw e
    } finally {
      isMessagesLoading.value = false
    }
  }

  // 4. 发送消息
  async function sendMessage(content: string) {
    if (!activeConversation.value || !content.trim()) return
    try {
      // 在发送前最后检查一次
      if (connection.value?.state !== signalR.HubConnectionState.Connected) {
        console.error('发送失败：连接已断开。')
        // 尝试重连
        await startConnection()
      }

      // 再次检查
      if (connection.value?.state === signalR.HubConnectionState.Connected) {
        await connection.value?.invoke(
          'SendMessage',
          activeConversation.value.conversationId,
          content,
        )
      } else {
        ElMessage.error('消息发送失败，连接已断开。')
      }
    } catch (e) {
      console.error('发送消息失败', e)
    }
  }

  async function openConversationById(conversationId: string) {
    // 如果已经在看这个会话了，就什么都不做
    if (activeConversation.value?.conversationId === conversationId) {
      return
    }

    isChatPanelVisible.value = true
    activeConversation.value = null
    isMessagesLoading.value = true
    try {
      // 1. 通过 HTTP GET 获取完整的会话历史
      const response = await apiClient.get<ConversationDto>(
        `${chatApiBaseUrl}/api/conversations/${conversationId}`,
      )
      activeConversation.value = response.data

      // 2. 确保 SignalR 已连接 (如果还没连的话)
      await startConnection()

      // 3. 加入 SignalR 组
      if (connection.value?.state === signalR.HubConnectionState.Connected) {
        await connection.value.invoke('JoinGroup', response.data.conversationId)
      }
    } catch (e) {
      console.error('加载会话失败', e)
      ElMessage.error('加载会话失败')
    } finally {
      isMessagesLoading.value = false
    }
  }

  // 5. UI 控制
  function showChatPanel() {
    isChatPanelVisible.value = true
  }
  function hideChatPanel() {
    isChatPanelVisible.value = false
  }
  function backToConversationList() {
    activeConversation.value = null
  }

  return {
    connection,
    conversations,
    activeConversation,
    isChatPanelVisible,
    isConversationsLoading,
    isMessagesLoading,
    activeParticipant,
    startConnection,
    openChatWithUser,
    sendMessage,
    openConversationById,
    showChatPanel,
    hideChatPanel,
    backToConversationList,
  }
})
