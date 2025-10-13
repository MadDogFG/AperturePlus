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
