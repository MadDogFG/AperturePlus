// src/types/activity.ts

export type RoleType = 'Photographer' | 'Model'
export type ParticipantStatus = 'Pending' | 'Approved' | 'Rejected'

// 角色需求
export interface RoleRequirement {
  role: RoleType
  quantity: number
}

// 参与者（详细信息）
export interface Participant {
  userId: string
  userName: string
  avatarUrl?: string // 用户头像，可选
  role: RoleType
  status: ParticipantStatus
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
