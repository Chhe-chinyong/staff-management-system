import type { Staff } from "@/lib/types"
import { GENDER_LABELS } from "@/lib/constants"
import { format } from "date-fns"

function prepareRows(staffList: Staff[]) {
  return staffList.map((s) => ({
    id: s.id,
    fullName: s.fullName,
    birthday: format(new Date(s.birthday), "yyyy-MM-dd"),
    gender: GENDER_LABELS[s.gender] ?? "Unknown",
  }))
}

export async function exportToExcel(
  staffList: Staff[],
  filename: string,
  labels: { sheetName: string }
) {
  const XLSX = await import("xlsx")
  const rows = prepareRows(staffList)
  const worksheet = XLSX.utils.json_to_sheet(rows)
  const workbook = XLSX.utils.book_new()
  XLSX.utils.book_append_sheet(workbook, worksheet, labels.sheetName)

  worksheet["!cols"] = [{ wch: 12 }, { wch: 30 }, { wch: 14 }, { wch: 10 }]

  XLSX.writeFile(workbook, `${filename}.xlsx`)
}

export async function exportToPdf(
  staffList: Staff[],
  filename: string,
  labels: { title: string; headers: string[] }
) {
  const { default: jsPDF } = await import("jspdf")
  const { default: autoTable } = await import("jspdf-autotable")

  const doc = new jsPDF()

  doc.setFontSize(16)
  doc.text(labels.title, 14, 20)

  const rows = prepareRows(staffList)
  const tableData = rows.map((r) => [r.id, r.fullName, r.birthday, r.gender])

  autoTable(doc, {
    head: [labels.headers],
    body: tableData,
    startY: 30,
    styles: { fontSize: 10 },
    headStyles: { fillColor: [59, 130, 246] },
  })

  doc.save(`${filename}.pdf`)
}
