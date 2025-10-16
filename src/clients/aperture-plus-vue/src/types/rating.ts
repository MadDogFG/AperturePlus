export interface ReceivedRating {
  ratingId: string
  activityId: string
  rateByUserId: string
  rateByUserName: string // 我们需要让后端在返回时，附带上评价人的名字
  score: number // 1-5 的分数
  comments: string
  ratedUserRole: 'Photographer' | 'Model'
  createdAt: string
}

export interface PendingRatingDto {
  ratingId: string // 这个对应后端 Rating 表的主键
  activityId: string
  activityTitle: string
  rateToUserId: string
  rateToUserName: string
  ratedUserRole: 'Photographer' | 'Model'
  // 为了让 v-model 能直接绑定，我们在前端为它补充这两个字段
  score?: number
  comments?: string
}
