"use client"

import { format } from "date-fns"
import { useTranslation } from "react-i18next"
import { PencilIcon, Trash2Icon, Loader2Icon } from "lucide-react"
import type { Staff } from "@/lib/types"
import { GENDER_LABELS } from "@/lib/constants"
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table"
import { Button } from "@/components/ui/button"
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select"

const PAGE_SIZE_OPTIONS = [5, 10, 25, 50]

interface StaffTableProps {
  staff: Staff[]
  isLoading: boolean
  page: number
  pageSize: number
  totalPages: number
  totalCount: number
  onPageChange: (page: number) => void
  onPageSizeChange: (size: number) => void
  onEdit: (staff: Staff) => void
  onDelete: (staff: Staff) => void
}

function StaffTable({
  staff,
  isLoading,
  page,
  pageSize,
  totalPages,
  totalCount,
  onPageChange,
  onPageSizeChange,
  onEdit,
  onDelete,
}: StaffTableProps) {
  const { t } = useTranslation()

  if (isLoading) {
    return (
      <div className="flex items-center justify-center py-20">
        <Loader2Icon className="size-6 animate-spin text-muted-foreground" />
      </div>
    )
  }

  if (staff.length === 0) {
    return (
      <div className="flex items-center justify-center py-20 text-muted-foreground">
        {t("staff.noStaff")}
      </div>
    )
  }

  return (
    <div>
      <div className="rounded-lg border">
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead className="w-[120px]">{t("staff.staffId")}</TableHead>
              <TableHead>{t("staff.fullName")}</TableHead>
              <TableHead className="w-[130px]">{t("staff.birthday")}</TableHead>
              <TableHead className="w-[100px]">{t("staff.gender")}</TableHead>
              <TableHead className="w-[100px] text-right">
                {t("staff.actions")}
              </TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {staff.map((s) => (
              <TableRow key={s.id}>
                <TableCell className="font-mono text-sm">{s.id}</TableCell>
                <TableCell>{s.fullName}</TableCell>
                <TableCell>
                  {format(new Date(s.birthday), "yyyy-MM-dd")}
                </TableCell>
                <TableCell>{GENDER_LABELS[s.gender] ?? "Unknown"}</TableCell>
                <TableCell className="text-right">
                  <div className="flex items-center justify-end gap-1">
                    <Button
                      variant="ghost"
                      size="icon"
                      onClick={() => onEdit(s)}
                      title={t("staff.edit")}
                    >
                      <PencilIcon className="size-4" />
                    </Button>
                    <Button
                      variant="ghost"
                      size="icon"
                      onClick={() => onDelete(s)}
                      title={t("staff.delete")}
                    >
                      <Trash2Icon className="size-4 text-destructive" />
                    </Button>
                  </div>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </div>

      <div className="flex items-center justify-between pt-4">
        <div className="flex items-center gap-2">
          <p className="text-sm text-muted-foreground">
            {totalCount} {t("staff.recordsTotal")}
          </p>
          <span className="text-sm text-muted-foreground">|</span>
          <div className="flex items-center gap-1.5">
            <span className="text-sm text-muted-foreground">
              {t("staff.rows")}
            </span>
            <Select
              value={String(pageSize)}
              onValueChange={(val) => onPageSizeChange(Number(val))}
            >
              <SelectTrigger className="h-8 w-[70px] text-sm">
                <SelectValue />
              </SelectTrigger>
              <SelectContent>
                {PAGE_SIZE_OPTIONS.map((size) => (
                  <SelectItem key={size} value={String(size)}>
                    {size}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </div>
        </div>
        <div className="flex items-center gap-2">
          <Button
            variant="outline"
            size="sm"
            disabled={page <= 1}
            onClick={() => onPageChange(page - 1)}
          >
            {t("pagination.previous")}
          </Button>
          <span className="text-sm text-muted-foreground">
            {t("pagination.page")} {page} {t("pagination.of")} {totalPages}
          </span>
          <Button
            variant="outline"
            size="sm"
            disabled={page >= totalPages}
            onClick={() => onPageChange(page + 1)}
          >
            {t("pagination.next")}
          </Button>
        </div>
      </div>
    </div>
  )
}

export { StaffTable }
