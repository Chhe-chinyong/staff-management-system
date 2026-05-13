"use client"

import { useState } from "react"
import { useTranslation } from "react-i18next"
import { format } from "date-fns"
import { CalendarIcon, RotateCcwIcon, SearchIcon } from "lucide-react"
import { type DateRange } from "react-day-picker"
import { GENDER_OPTIONS } from "@/lib/constants"
import type { StaffSearchParams } from "@/lib/types"
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
import { Separator } from "@/components/ui/separator"
import { cn } from "@/lib/utils"

interface AdvancedSearchFormProps {
  onSearch: (params: StaffSearchParams) => void
  onReset: () => void
}

function AdvancedSearchForm({ onSearch, onReset }: AdvancedSearchFormProps) {
  const { t } = useTranslation()
  const [staffId, setStaffId] = useState("")
  const [fullName, setFullName] = useState("")
  const [gender, setGender] = useState<string>("")
  const [dateRange, setDateRange] = useState<DateRange | undefined>()

  const displayText = dateRange?.from
    ? dateRange.to
      ? `${format(dateRange.from, "yyyy-MM-dd")} — ${format(dateRange.to, "yyyy-MM-dd")}`
      : format(dateRange.from, "yyyy-MM-dd")
    : t("advancedSearch.pickDateRange")

  function handleSearch() {
    const params: StaffSearchParams = {
      staffId: staffId || undefined,
      fullName: fullName || undefined,
      gender: gender ? Number(gender) : undefined,
      birthdayFrom: dateRange?.from
        ? format(dateRange.from, "yyyy-MM-dd")
        : undefined,
      birthdayTo: dateRange?.to
        ? format(dateRange.to, "yyyy-MM-dd")
        : undefined,
      page: 1,
    }
    onSearch(params)
  }

  function handleReset() {
    setStaffId("")
    setFullName("")
    setGender("")
    setDateRange(undefined)
    onReset()
  }

  return (
    <div className="space-y-4 rounded-lg border p-4">
      <h3 className="text-sm font-medium">{t("advancedSearch.title")}</h3>
      <Separator />
      <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-4">
        <div className="grid gap-2">
          <Label htmlFor="search-staffId">{t("advancedSearch.staffId")}</Label>
          <Input
            id="search-staffId"
            placeholder={t("advancedSearch.staffIdPlaceholder")}
            value={staffId}
            onChange={(e) => setStaffId(e.target.value)}
          />
        </div>

        <div className="grid gap-2">
          <Label htmlFor="search-fullName">{t("staff.fullName")}</Label>
          <Input
            id="search-fullName"
            placeholder={t("advancedSearch.fullNamePlaceholder")}
            value={fullName}
            onChange={(e) => setFullName(e.target.value)}
          />
        </div>

        <div className="grid gap-2">
          <Label>{t("advancedSearch.gender")}</Label>
          <Select value={gender} onValueChange={setGender}>
            <SelectTrigger>
              <SelectValue placeholder={t("advancedSearch.all")} />
            </SelectTrigger>
            <SelectContent>
              <SelectItem value="0">{t("advancedSearch.all")}</SelectItem>
              {GENDER_OPTIONS.map((opt) => (
                <SelectItem key={opt.value} value={String(opt.value)}>
                  {opt.label}
                </SelectItem>
              ))}
            </SelectContent>
          </Select>
        </div>

        <div className="grid gap-2">
          <Label>{t("advancedSearch.birthdayRange")}</Label>
          <Popover>
            <PopoverTrigger asChild>
              <Button
                variant="outline"
                className={cn(
                  "w-full justify-start text-left font-normal",
                  !dateRange?.from && "text-muted-foreground"
                )}
              >
                <CalendarIcon className="mr-2 size-4 shrink-0" />
                <span className="truncate">{displayText}</span>
              </Button>
            </PopoverTrigger>
            <PopoverContent className="w-auto p-0" align="start">
              <Calendar
                mode="range"
                selected={dateRange}
                onSelect={setDateRange}
                numberOfMonths={2}
                captionLayout="dropdown"
                startMonth={new Date(1950, 0)}
                endMonth={new Date()}
                defaultMonth={dateRange?.from ?? new Date(1990, 0)}
              />
            </PopoverContent>
          </Popover>
        </div>
      </div>

      <div className="flex gap-2">
        <Button onClick={handleSearch} size="sm">
          <SearchIcon className="mr-2 size-4" />
          {t("advancedSearch.search")}
        </Button>
        <Button variant="outline" onClick={handleReset} size="sm">
          <RotateCcwIcon className="mr-2 size-4" />
          {t("advancedSearch.reset")}
        </Button>
      </div>
    </div>
  )
}

export { AdvancedSearchForm }
