"use client"

import { useState } from "react"
import { useTranslation } from "react-i18next"
import { FileSpreadsheetIcon, FileTextIcon, Loader2Icon } from "lucide-react"
import type { Staff } from "@/lib/types"
import { Button } from "@/components/ui/button"

interface ExportButtonsProps {
  staff: Staff[]
  disabled?: boolean
}

function ExportButtons({ staff, disabled }: ExportButtonsProps) {
  const { t } = useTranslation()
  const [exportingExcel, setExportingExcel] = useState(false)
  const [exportingPdf, setExportingPdf] = useState(false)

  async function handleExportExcel() {
    setExportingExcel(true)
    try {
      const { exportToExcel } = await import("@/lib/export-utils")
      await exportToExcel(staff, `staff-report-${Date.now()}`, {
        sheetName: t("exportUtils.staff"),
      })
    } catch (err) {
      console.error("Excel export failed:", err)
    } finally {
      setExportingExcel(false)
    }
  }

  async function handleExportPdf() {
    setExportingPdf(true)
    try {
      const { exportToPdf } = await import("@/lib/export-utils")
      await exportToPdf(staff, `staff-report-${Date.now()}`, {
        title: t("exportUtils.staffReport"),
        headers: [
          t("staff.staffId"),
          t("staff.fullName"),
          t("staff.birthday"),
          t("staff.gender"),
        ],
      })
    } catch (err) {
      console.error("PDF export failed:", err)
    } finally {
      setExportingPdf(false)
    }
  }

  const isDisabled = disabled || staff.length === 0

  return (
    <div className="flex gap-2">
      <Button
        variant="outline"
        size="sm"
        onClick={handleExportExcel}
        disabled={isDisabled || exportingExcel}
      >
        {exportingExcel ? (
          <Loader2Icon className="mr-2 size-4 animate-spin" />
        ) : (
          <FileSpreadsheetIcon className="mr-2 size-4" />
        )}
        {t("export.excel")}
      </Button>
      <Button
        variant="outline"
        size="sm"
        onClick={handleExportPdf}
        disabled={isDisabled || exportingPdf}
      >
        {exportingPdf ? (
          <Loader2Icon className="mr-2 size-4 animate-spin" />
        ) : (
          <FileTextIcon className="mr-2 size-4" />
        )}
        {t("export.pdf")}
      </Button>
    </div>
  )
}

export { ExportButtons }
