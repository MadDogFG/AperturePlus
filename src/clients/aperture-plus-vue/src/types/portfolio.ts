// src/types/portfolio.ts

export interface Photo {
  photoId: string
  photoUrl: string
  tags: string[]
  uploadedAt: string
}

export interface Gallery {
  galleryId: string
  galleryName: string
  coverPhotoUrl?: string
  photos: Photo[]
  createdAt: string
}

export interface Portfolio {
  portfolioId: string
  userId: string
  galleries: Gallery[]
}
