export interface Staff {
  id: string
  fullName: string
  birthday: string
  gender: 1 | 2
  genderName?: string
}

export interface StaffFormData {
  id: string
  fullName: string
  birthday: string
  gender: 1 | 2
}

export interface StaffSearchParams {
  staffId?: string
  fullName?: string
  gender?: number
  birthdayFrom?: string
  birthdayTo?: string
  page?: number
  pageSize?: number
}

export interface PaginatedResponse<T> {
  items: T[]
  totalCount: number
  page: number
  pageSize: number
  totalPages: number
  hasPreviousPage: boolean
  hasNextPage: boolean
}

export interface ApiResponse<T> {
  success: boolean
  data: T
  message?: string
}
