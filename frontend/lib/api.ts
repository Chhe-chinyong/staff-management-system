import type {
  Staff,
  StaffFormData,
  StaffSearchParams,
  PaginatedResponse,
} from "@/lib/types"
import { API_BASE_URL, DEFAULT_PAGE_SIZE } from "@/lib/constants"
import { apiClient } from "@/lib/api-client"

function request<T>(endpoint: string, options?: RequestInit): Promise<T> {
  return apiClient.request<T>(`${API_BASE_URL}${endpoint}`, options)
}

export async function getStaffList(
  page = 1,
  pageSize = DEFAULT_PAGE_SIZE
): Promise<PaginatedResponse<Staff>> {
  return request<PaginatedResponse<Staff>>(
    `/Staffs?page=${page}&pageSize=${pageSize}`
  )
}

export async function getStaffById(id: string): Promise<Staff> {
  return request<Staff>(`/Staffs/${encodeURIComponent(id)}`)
}

export async function createStaff(data: StaffFormData): Promise<Staff> {
  return request<Staff>("/Staffs", {
    method: "POST",
    body: JSON.stringify(data),
  })
}

export async function updateStaff(
  id: string,
  data: StaffFormData
): Promise<Staff> {
  const { fullName, birthday, gender } = data
  return request<Staff>(`/Staffs/${encodeURIComponent(id)}`, {
    method: "PUT",
    body: JSON.stringify({ fullName, birthday, gender }),
  })
}

export async function deleteStaff(id: string): Promise<void> {
  await request<void>(`/Staffs/${encodeURIComponent(id)}`, {
    method: "DELETE",
  })
}

export async function searchStaff(
  params: StaffSearchParams
): Promise<PaginatedResponse<Staff>> {
  const searchParams = new URLSearchParams()

  if (params.staffId) searchParams.set("StaffId", params.staffId)
  if (params.fullName) searchParams.set("FullName", params.fullName)
  if (params.gender !== undefined && params.gender !== 0)
    searchParams.set("Gender", String(params.gender))
  if (params.birthdayFrom) searchParams.set("BirthdayFrom", params.birthdayFrom)
  if (params.birthdayTo) searchParams.set("BirthdayTo", params.birthdayTo)
  if (params.page) searchParams.set("Page", String(params.page))
  if (params.pageSize) searchParams.set("PageSize", String(params.pageSize))

  const qs = searchParams.toString()
  return request<PaginatedResponse<Staff>>(
    `/Staffs/search${qs ? `?${qs}` : ""}`
  )
}

export function buildSearchKey(params: StaffSearchParams): string {
  return [
    "/Staffs/search",
    params.staffId ?? "",
    params.fullName ?? "",
    params.gender ?? "",
    params.birthdayFrom ?? "",
    params.birthdayTo ?? "",
    String(params.page ?? 1),
    String(params.pageSize ?? DEFAULT_PAGE_SIZE),
  ].join("|")
}
