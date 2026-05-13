"use client"

import { useEffect, useState } from "react"

function I18nProvider({ children }: { children: React.ReactNode }) {
  const [ready, setReady] = useState(false)

  useEffect(() => {
    const stored = localStorage.getItem("lang")
    if (stored) {
      document.documentElement.lang = stored
    }
    import("@/lib/i18n").then(() => setReady(true))
  }, [])

  if (!ready) return null

  return <>{children}</>
}

export default I18nProvider
