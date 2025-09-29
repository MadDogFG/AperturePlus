// src/types/activity.ts

export type RoleType = 'Photographer' | 'Model'
export type ParticipantStatus = 'Pending' | 'Approved' | 'Rejected'

// 角色需求
export interface RoleRequirement {
  role: number // 0 for Photographer, 1 for Model
  quantity: number
}

// 参与者（详细信息）
export interface Participant {
  userId: string
  userName: string // 我们将通过API调用来填充这个字段
  avatarUrl?: string // 这个也是
  role: number | RoleType // 允许是数字或字符串
  status: ParticipantStatus
  apliedAt?: string // apliedAt 是拼写错误，但我们遵循后端
}

// 活动列表项（我们之前定义好的）
export interface Activity {
  activityId: string
  activityTitle: string
  activityDescription: string
  activityLocation: {
    latitude: number
    longitude: number
  }
  activityStartTime: string
  postedByUser: {
    userId: string
    userName: string
  }
  status: string
  fee: number
  roleRequirements: RoleRequirement[]
  totalRequiredCount: number
  approvedParticipantsCount: number
  pendingParticipantsCount: number
}

// 活动详细信息（用于详情页，包含完整的参与者列表）
export interface ActivityDetail {
  activityId: string
  activityTitle: string
  activityDescription: string
  activityLocation: {
    latitude: number
    longitude: number
  }
  activityStartTime: string
  postedByUser: {
    userId: string
    userName: string
  }
  status: string
  fee: number
  roleRequirements: RoleRequirement[]
  participants: Participant[]
}
