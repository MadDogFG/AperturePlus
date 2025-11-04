<template>
  <div
    class="chat-fab"
    v-if="!chatStore.isChatPanelVisible && authStore.isAuthenticated"
    @click="chatStore.showChatPanel"
  >
    <el-icon><ChatDotRound /></el-icon>
  </div>

  <div class="chat-window" v-if="chatStore.isChatPanelVisible && authStore.isAuthenticated">
    <header class="chat-header">
      <el-button
        v-if="chatStore.activeConversation"
        :icon="ArrowLeftBold"
        circle
        @click="chatStore.backToConversationList"
      />
      <div class="header-title">
        <span v-if="!chatStore.activeConversation">我的私信</span>
        <span v-else-if="chatStore.activeParticipant">
          {{ chatStore.activeParticipant.userName }}
        </span>
      </div>
      <el-button :icon="CloseBold" circle @click="chatStore.hideChatPanel" />
    </header>

    <div class="conversation-list" v-if="!chatStore.activeConversation">
      <div v-if="chatStore.isConversationsLoading" class="loading-state">
        <el-skeleton :rows="3" animated />
      </div>
      <div
        v-for="convo in chatStore.conversations"
        :key="convo.conversationId"
        class="convo-item"
        @click="chatStore.openConversationById(convo.conversationId)"
      >
        <el-avatar :size="40" :src="convo.participant.avatarUrl" />
        <div class="convo-info">
          <span class="username">{{ convo.participant.userName }}</span>
          <span class="last-message">
            {{ convo.lastMessage?.content || '...' }}
          </span>
        </div>
        <span class="timestamp">{{ formatTimestamp(convo.lastUpdatedAt) }}</span>
      </div>
      <el-empty
        v-if="!chatStore.isConversationsLoading && chatStore.conversations.length === 0"
        description="暂无会话"
        :image-size="80"
      />
    </div>

    <div class="message-container" v-else>
      <div v-if="chatStore.isMessagesLoading" class="loading-state">
        <el-skeleton :rows="5" animated />
      </div>
      <div class="message-list" ref="messageListEl">
        <div
          v-for="msg in chatStore.activeConversation.messages"
          :key="msg.messageId"
          class="message-row"
          :class="{ 'my-message': msg.senderId === userStore.profile?.userId }"
        >
          <div class="message-bubble">{{ msg.content }}</div>
        </div>
      </div>
      <div class="message-input">
        <el-input
          v-model="newMessage"
          placeholder="输入消息..."
          @keydown.enter.prevent="handleSendMessage"
        >
          <template #append>
            <el-button @click="handleSendMessage">发送</el-button>
          </template>
        </el-input>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, watch, nextTick } from 'vue'
import { useChatStore } from '@/stores/chat'
import { useAuthStore } from '@/stores/auth'
import { useUserStore } from '@/stores/user'
import { ArrowLeftBold, CloseBold, ChatDotRound } from '@element-plus/icons-vue'

const chatStore = useChatStore()
const authStore = useAuthStore()
const userStore = useUserStore()
const newMessage = ref('')
const messageListEl = ref<HTMLDivElement | null>(null)

// 格式化时间戳
function formatTimestamp(dateString: string) {
  const date = new Date(dateString)
  // (您可以添加更复杂的时间格式化逻辑, e.g., "昨天 10:30")
  return date.toLocaleTimeString('zh-CN', { hour: '2-digit', minute: '2-digit' })
}

// 滚动到底部
function scrollToBottom() {
  nextTick(() => {
    if (messageListEl.value) {
      messageListEl.value.scrollTop = messageListEl.value.scrollHeight
    }
  })
}

// 监听新消息并滚动
watch(
  () => chatStore.activeConversation?.messages.length,
  () => {
    scrollToBottom()
  },
)

// 打开会话时滚动
watch(
  () => chatStore.activeConversation,
  (newConvo) => {
    if (newConvo) {
      scrollToBottom()
    }
  },
)

// 发送消息
const handleSendMessage = () => {
  chatStore.sendMessage(newMessage.value)
  newMessage.value = ''
}
</script>

<style scoped>
.chat-fab {
  position: fixed;
  bottom: 30px;
  right: 30px;
  width: 60px;
  height: 60px;
  border-radius: 50%;
  background-color: var(--el-color-primary);
  color: white;
  display: flex;
  justify-content: center;
  align-items: center;
  font-size: 30px;
  cursor: pointer;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
  z-index: 100;
}

.chat-window {
  position: fixed;
  bottom: 30px;
  right: 30px;
  width: 370px;
  height: 500px;
  background: #fff;
  border-radius: 8px;
  box-shadow: 0 5px 20px rgba(0, 0, 0, 0.2);
  display: flex;
  flex-direction: column;
  overflow: hidden;
  z-index: 101;
}

.chat-header {
  display: flex;
  align-items: center;
  padding: 10px 15px;
  border-bottom: 1px solid #ebeef5;
  flex-shrink: 0;
}

.header-title {
  flex-grow: 1;
  text-align: center;
  font-weight: 600;
  font-size: 1.1rem;
}

.conversation-list {
  flex-grow: 1;
  overflow-y: auto;
}

.convo-item {
  display: flex;
  align-items: center;
  padding: 12px 15px;
  cursor: pointer;
  gap: 10px;
}

.convo-item:hover {
  background-color: #f5f7fa;
}

.convo-info {
  flex-grow: 1;
  display: flex;
  flex-direction: column;
  overflow: hidden;
}

.username {
  font-weight: 500;
}

.last-message {
  font-size: 0.85rem;
  color: #909399;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.timestamp {
  font-size: 0.75rem;
  color: #c0c4cc;
  flex-shrink: 0;
}

.message-container {
  flex-grow: 1;
  display: flex;
  flex-direction: column;
  overflow: hidden;
}

.message-list {
  flex-grow: 1;
  overflow-y: auto;
  padding: 15px;
}

.message-row {
  display: flex;
  margin-bottom: 10px;
}

.message-bubble {
  padding: 8px 12px;
  border-radius: 18px;
  background-color: #f0f2f5;
  color: #303133;
  max-width: 70%;
}

.my-message {
  justify-content: flex-end;
}

.my-message .message-bubble {
  background-color: var(--el-color-primary);
  color: white;
}

.message-input {
  padding: 10px;
  border-top: 1px solid #ebeef5;
}

.loading-state {
  padding: 20px;
}
</style>
