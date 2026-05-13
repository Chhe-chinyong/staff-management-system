"use client"

import { useState } from "react"
import { useTranslation } from "react-i18next"
import { Loader2Icon } from "lucide-react"
import type { Staff } from "@/lib/types"
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog"
import { Button } from "@/components/ui/button"

interface StaffDeleteDialogProps {
  open: boolean
  onOpenChange: (open: boolean) => void
  staff: Staff | null
  onConfirm: (staffId: string) => Promise<void>
}

function StaffDeleteDialog({
  open,
  onOpenChange,
  staff,
  onConfirm,
}: StaffDeleteDialogProps) {
  const { t } = useTranslation()
  const [deleting, setDeleting] = useState(false)

  async function handleConfirm() {
    if (!staff) return
    setDeleting(true)
    try {
      await onConfirm(staff.id)
      onOpenChange(false)
    } finally {
      setDeleting(false)
    }
  }

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-[400px]">
        <DialogHeader>
          <DialogTitle>{t("deleteDialog.title")}</DialogTitle>
          <DialogDescription>
            {t("deleteDialog.description")}{" "}
            <span className="font-semibold">{staff?.fullName}</span> (
            {staff?.id})? {t("deleteDialog.cannotUndo")}
          </DialogDescription>
        </DialogHeader>
        <DialogFooter>
          <Button
            variant="outline"
            onClick={() => onOpenChange(false)}
            disabled={deleting}
          >
            {t("form.cancel")}
          </Button>
          <Button
            variant="destructive"
            onClick={handleConfirm}
            disabled={deleting}
          >
            {deleting && <Loader2Icon className="mr-2 size-4 animate-spin" />}
            {t("staff.delete")}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  )
}

export { StaffDeleteDialog }
