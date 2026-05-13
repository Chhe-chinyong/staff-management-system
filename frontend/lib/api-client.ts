import { toast } from "sonner"

export interface ApiError {
  message?: string
  status?: number
}

type ResponseInterceptor = (
  response: Response,
  error: ApiError | null
) => ApiError | null

class ApiClient {
  private responseInterceptors: ResponseInterceptor[] = []

  addResponseInterceptor(interceptor: ResponseInterceptor) {
    this.responseInterceptors.push(interceptor)
    return () => {
      this.responseInterceptors = this.responseInterceptors.filter(
        (i) => i !== interceptor
      )
    }
  }

  async request<T>(url: string, options?: RequestInit): Promise<T> {
    const hasBody = options?.body != null
    const headers: Record<string, string> = {}
    if (hasBody) headers["Content-Type"] = "application/json"
    if (options?.headers) {
      const optHeaders = options.headers as Record<string, string>
      Object.assign(headers, optHeaders)
    }

    const response = await fetch(url, {
      ...options,
      headers,
    })

    if (!response.ok) {
      const body = await response.json().catch(() => ({}))
      const error: ApiError = {
        message:
          (body as { message?: string }).message ||
          `Request failed with status ${response.status}`,
        status: response.status,
      }

      let intercepted: ApiError | null = error
      for (const interceptor of this.responseInterceptors) {
        intercepted = interceptor(response, intercepted)
      }

      throw new Error(intercepted?.message ?? "Request failed")
    }

    if (response.status === 204) return undefined as T

    const text = await response.text()
    if (!text) return undefined as T
    return JSON.parse(text) as T
  }
}

export const apiClient = new ApiClient()

function getErrorToastConfig(status: number): {
  fn: (typeof toast)["error" | "warning" | "info"]
  fallback: string
} {
  if (status === 401) return { fn: toast.warning, fallback: "Unauthorized" }
  if (status === 403) return { fn: toast.warning, fallback: "Access denied" }
  if (status === 404) return { fn: toast.info, fallback: "Resource not found" }
  if (status === 408 || status === 504)
    return { fn: toast.warning, fallback: "Request timed out" }
  if (status === 409)
    return { fn: toast.warning, fallback: "Conflict detected" }
  if (status === 422)
    return { fn: toast.warning, fallback: "Validation failed" }
  if (status === 429)
    return { fn: toast.warning, fallback: "Too many requests" }
  if (status >= 500)
    return {
      fn: toast.error,
      fallback: "Server error. Please try again later.",
    }
  if (status >= 400) return { fn: toast.warning, fallback: "Bad request" }
  return { fn: toast.error, fallback: "An unexpected error occurred" }
}

apiClient.addResponseInterceptor((_response, error) => {
  if (error) {
    const config = getErrorToastConfig(error.status ?? 0)
    config.fn(error.message ?? config.fallback)
  }
  return error
})
