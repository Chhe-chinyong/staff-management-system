"use client"

import { useEffect, useRef } from "react"
import useSWR from "swr"
import useSWRMutation from "swr/mutation"
import { toast } from "sonner"
import type {
  Staff,
  StaffFormData,
  StaffSearchParams,
  PaginatedResponse,
} from "@/lib/types"
import {
  createStaff,
  updateStaff,
  deleteStaff,
  searchStaff,
  buildSearchKey,
} from "@/lib/api"
import { apiClient } from "@/lib/api-client"
import { API_BASE_URL, DEFAULT_PAGE_SIZE } from "@/lib/constants"
import { MOCK_STAFF } from "@/lib/mock-data"

const USE_MOCK = false

function mockPaginate(
  staff: Staff[],
  page: number,
  pageSize: number
): PaginatedResponse<Staff> {
  const start = (page - 1) * pageSize
  const items = staff.slice(start, start + pageSize)
  return {
    items,
    totalCount: staff.length,
    page,
    pageSize,
    totalPages: Math.ceil(staff.length / pageSize),
    hasPreviousPage: page > 1,
    hasNextPage: page < Math.ceil(staff.length / pageSize),
  }
}

function mockSearch(params: StaffSearchParams): PaginatedResponse<Staff> {
  let filtered = [...MOCK_STAFF]

  if (params.staffId) {
    filtered = filtered.filter((s) =>
      s.id.toLowerCase().includes(params.staffId!.toLowerCase())
    )
  }
  if (params.fullName) {
    filtered = filtered.filter((s) =>
      s.fullName.toLowerCase().includes(params.fullName!.toLowerCase())
    )
  }
  if (params.gender !== undefined && params.gender !== 0) {
    filtered = filtered.filter((s) => s.gender === params.gender)
  }
  if (params.birthdayFrom) {
    filtered = filtered.filter((s) => s.birthday >= params.birthdayFrom!)
  }
  if (params.birthdayTo) {
    filtered = filtered.filter((s) => s.birthday <= params.birthdayTo!)
  }

  return mockPaginate(
    filtered,
    params.page ?? 1,
    params.pageSize ?? DEFAULT_PAGE_SIZE
  )
}

function fetcher<T>(url: string): Promise<T> {
  return apiClient.request<T>(`${API_BASE_URL}${url.replace(/^\/api/, "")}`)
}

function useSWRErrorToast(error: Error | undefined) {
  const lastShown = useRef<string | null>(null)
  useEffect(() => {
    if (error && error.message !== lastShown.current) {
      lastShown.current = error.message
      toast.error(error.message)
    }
    if (!error) {
      lastShown.current = null
    }
  }, [error])
}

export function useStaffList(page = 1, pageSize = DEFAULT_PAGE_SIZE) {
  const key = `/api/Staffs?page=${page}&pageSize=${pageSize}`

  const { data, error, isLoading, mutate } = useSWR<PaginatedResponse<Staff>>(
    key,
    USE_MOCK
      ? () => Promise.resolve(mockPaginate(MOCK_STAFF, page, pageSize))
      : fetcher,
    {
      revalidateOnFocus: false,
    }
  )

  useSWRErrorToast(error)

  return {
    data: data?.items ?? [],
    totalCount: data?.totalCount ?? 0,
    totalPages: data?.totalPages ?? 0,
    page: data?.page ?? page,
    error,
    isLoading,
    mutate,
  }
}

export function useStaffSearch(params: StaffSearchParams, enabled = true) {
  const key = enabled ? buildSearchKey(params) : null

  const { data, error, isLoading, mutate } = useSWR<PaginatedResponse<Staff>>(
    key,
    USE_MOCK
      ? () => Promise.resolve(mockSearch(params))
      : () => searchStaff(params),
    {
      revalidateOnFocus: false,
    }
  )

  useSWRErrorToast(error)

  return {
    data: data?.items ?? [],
    totalCount: data?.totalCount ?? 0,
    totalPages: data?.totalPages ?? 0,
    page: data?.page ?? params.page ?? 1,
    error,
    isLoading,
    mutate,
  }
}

export function useCreateStaff() {
  return useSWRMutation<Staff, Error, string, StaffFormData>(
    "/api/Staffs",
    async (_url, { arg }) => createStaff(arg)
  )
}

export function useUpdateStaff() {
  return useSWRMutation<
    Staff,
    Error,
    string,
    { id: string; data: StaffFormData }
  >("/api/Staffs", async (_url, { arg }) => updateStaff(arg.id, arg.data))
}

export function useDeleteStaff() {
  return useSWRMutation<void, Error, string, string>(
    "/api/Staffs",
    async (_url, { arg }) => deleteStaff(arg)
  )
}
