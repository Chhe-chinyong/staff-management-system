export const GENDER_OPTIONS = [
  { value: 1, label: "Male" },
  { value: 2, label: "Female" },
] as const

export const GENDER_LABELS: Record<number, string> = {
  1: "Male",
  2: "Female",
}

export const DEFAULT_PAGE_SIZE = 10

export const API_BASE_URL =
  process.env.NEXT_PUBLIC_API_URL || "http://localhost:5001/api"
