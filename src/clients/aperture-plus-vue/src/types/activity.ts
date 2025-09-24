// src/types/activity.ts
export interface Activity {
  activityId: string
  activityTitle: string
  activityDescription: string
  activityLocation: {
    latitude: number
    longitude: number
  }
  activityStartTime: string // 日期我们先用字符串接收
  postedByUser: {
    userId: string
    userName: string
  }
  status: string
  fee: number
  // roleRequirements 和 participants 暂时先不加，等需要时再细化
}
