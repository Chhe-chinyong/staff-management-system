"use client"

import { useEffect, useState } from "react"
import { useTranslation } from "react-i18next"
import { format, parseISO } from "date-fns"
import { CalendarIcon, Loader2Icon } from "lucide-react"
import type { Staff, StaffFormData } from "@/lib/types"
import { GENDER_OPTIONS } from "@/lib/constants"
import { staffFormSchema, type StaffFormValues } from "@/lib/validations"
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select"
import { Calendar } from "@/components/ui/calendar"
import {
  Popover,
  PopoverContent,
  PopoverTrigger,
} from "@/components/ui/popover"
import { cn } from "@/lib/utils"

interface StaffFormDialogProps {
  open: boolean
  onOpenChange: (open: boolean) => void
  staff?: Staff | null
  onSubmit: (data: StaffFormData) => Promise<void>
}

const EMPTY_FORM: StaffFormValues = {
  id: "",
  fullName: "",
  birthday: "",
  gender: 1,
}

function StaffFormDialog({
  open,
  onOpenChange,
  staff,
  onSubmit,
}: StaffFormDialogProps) {
  const { t } = useTranslation()
  const isEdit = !!staff
  const [form, setForm] = useState<StaffFormValues>(EMPTY_FORM)
  const [errors, setErrors] = useState<Record<string, string>>({})
  const [submitting, setSubmitting] = useState(false)

  function resetForm() {
    setForm(EMPTY_FORM)
    setErrors({})
  }

  function handleOpenChange(value: boolean) {
    if (!value) {
      resetForm()
    }
    onOpenChange(value)
  }

  useEffect(() => {
    if (open) {
      if (staff) {
        setForm({
          id: staff.id,
          fullName: staff.fullName,
          birthday: staff.birthday,
          gender: staff.gender,
        })
      } else {
        resetForm()
      }
      setErrors({})
    }
  }, [open, staff])

  function validate(): boolean {
    const result = staffFormSchema.safeParse(form)
    if (!result.success) {
      const fieldErrors: Record<string, string> = {}
      for (const issue of result.error.issues) {
        const key = issue.path[0]
        if (typeof key === "string" && !fieldErrors[key]) {
          fieldErrors[key] = t(issue.message)
        }
      }
      setErrors(fieldErrors)
      return false
    }
    setErrors({})
    return true
  }

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault()
    if (!validate()) return

    setSubmitting(true)
    try {
      await onSubmit({
        id: form.id,
        fullName: form.fullName,
        birthday: form.birthday,
        gender: form.gender as 1 | 2,
      })
      resetForm()
      onOpenChange(false)
    } finally {
      setSubmitting(false)
    }
  }

  function updateField<K extends keyof StaffFormValues>(
    key: K,
    value: StaffFormValues[K]
  ) {
    setForm((prev) => ({ ...prev, [key]: value }))
    if (errors[key]) {
      setErrors((prev) => {
        const next = { ...prev }
        delete next[key]
        return next
      })
    }
  }

  return (
    <Dialog open={open} onOpenChange={handleOpenChange}>
      <DialogContent className="sm:max-w-[460px]">
        <DialogHeader>
          <DialogTitle>
            {isEdit ? t("form.editTitle") : t("form.addTitle")}
          </DialogTitle>
          <DialogDescription>
            {isEdit ? t("form.editDescription") : t("form.addDescription")}
          </DialogDescription>
        </DialogHeader>

        <form onSubmit={handleSubmit} className="grid gap-4 py-4">
          <div className="grid gap-2">
            <Label htmlFor="staffId">{t("staff.staffId")}</Label>
            <Input
              id="staffId"
              placeholder={t("form.staffIdPlaceholderNew")}
              maxLength={8}
              value={form.id}
              onChange={(e) => updateField("id", e.target.value)}
              disabled={isEdit}
              className={cn(errors.id && "border-destructive")}
            />
            {errors.id && (
              <p className="text-sm text-destructive">{errors.id}</p>
            )}
          </div>

          <div className="grid gap-2">
            <Label htmlFor="fullName">{t("staff.fullName")}</Label>
            <Input
              id="fullName"
              placeholder={t("form.fullNamePlaceholder")}
              maxLength={100}
              value={form.fullName}
              onChange={(e) => updateField("fullName", e.target.value)}
              className={cn(errors.fullName && "border-destructive")}
            />
            {errors.fullName && (
              <p className="text-sm text-destructive">{errors.fullName}</p>
            )}
          </div>

          <div className="grid gap-2">
            <Label>{t("staff.birthday")}</Label>
            <Popover>
              <PopoverTrigger asChild>
                <Button
                  variant="outline"
                  className={cn(
                    "w-full justify-start text-left font-normal",
                    !form.birthday && "text-muted-foreground",
                    errors.birthday && "border-destructive"
                  )}
                >
                  <CalendarIcon className="mr-2 size-4" />
                  {form.birthday
                    ? format(parseISO(form.birthday), "yyyy-MM-dd")
                    : t("form.pickDate")}
                </Button>
              </PopoverTrigger>
              <PopoverContent className="w-auto p-0" align="start">
                <Calendar
                  mode="single"
                  selected={form.birthday ? parseISO(form.birthday) : undefined}
                  onSelect={(date) =>
                    updateField(
                      "birthday",
                      date ? format(date, "yyyy-MM-dd") : ""
                    )
                  }
                  defaultMonth={
                    form.birthday ? parseISO(form.birthday) : new Date(1990, 0)
                  }
                  captionLayout="dropdown"
                  startMonth={new Date(1950, 0)}
                  endMonth={new Date()}
                />
              </PopoverContent>
            </Popover>
            {errors.birthday && (
              <p className="text-sm text-destructive">{errors.birthday}</p>
            )}
          </div>

          <div className="grid gap-2">
            <Label>{t("staff.gender")}</Label>
            <Select
              value={form.gender ? String(form.gender) : undefined}
              onValueChange={(val) =>
                updateField("gender", Number(val) as 1 | 2)
              }
            >
              <SelectTrigger
                className={cn(errors.gender && "border-destructive")}
              >
                <SelectValue placeholder={t("form.selectGender")} />
              </SelectTrigger>
              <SelectContent>
                {GENDER_OPTIONS.map((opt) => (
                  <SelectItem key={opt.value} value={String(opt.value)}>
                    {opt.label}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
            {errors.gender && (
              <p className="text-sm text-destructive">{errors.gender}</p>
            )}
          </div>

          <DialogFooter>
            <Button
              type="button"
              variant="outline"
              onClick={() => handleOpenChange(false)}
              disabled={submitting}
            >
              {t("form.cancel")}
            </Button>
            <Button type="submit" disabled={submitting}>
              {submitting && (
                <Loader2Icon className="mr-2 size-4 animate-spin" />
              )}
              {isEdit ? t("form.update") : t("form.create")}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  )
}

export { StaffFormDialog }
