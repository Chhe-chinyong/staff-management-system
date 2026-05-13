"use client"

import { useState } from "react"
import { useTranslation } from "react-i18next"
import {
  MoonIcon,
  PlusIcon,
  SearchIcon,
  SlidersHorizontalIcon,
  SunIcon,
} from "lucide-react"
import { toast } from "sonner"
import { useTheme } from "next-themes"
import type { Staff, StaffFormData, StaffSearchParams } from "@/lib/types"
import { DEFAULT_PAGE_SIZE } from "@/lib/constants"
import {
  useStaffList,
  useStaffSearch,
  useCreateStaff,
  useUpdateStaff,
  useDeleteStaff,
} from "@/hooks/use-staff"
import { StaffTable } from "@/components/staff/staff-table"
import { StaffFormDialog } from "@/components/staff/staff-form-dialog"
import { StaffDeleteDialog } from "@/components/staff/staff-delete-dialog"
import { AdvancedSearchForm } from "@/components/staff/advanced-search-form"
import { ExportButtons } from "@/components/staff/export-buttons"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Separator } from "@/components/ui/separator"
import { LanguageToggle } from "@/components/language-toggle"

export default function StaffManagementPage() {
  const { t } = useTranslation()
  const { resolvedTheme, setTheme } = useTheme()
  const [page, setPage] = useState(1)
  const [pageSize, setPageSize] = useState(DEFAULT_PAGE_SIZE)
  const [quickSearch, setQuickSearch] = useState("")
  const [showAdvancedSearch, setShowAdvancedSearch] = useState(false)
  const [searchParams, setSearchParams] = useState<StaffSearchParams | null>(
    null
  )

  const [formDialogOpen, setFormDialogOpen] = useState(false)
  const [editingStaff, setEditingStaff] = useState<Staff | null>(null)
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false)
  const [deletingStaff, setDeletingStaff] = useState<Staff | null>(null)

  const listData = useStaffList(page, pageSize)
  const searchData = useStaffSearch(
    searchParams ? { ...searchParams, page, pageSize } : { page, pageSize },
    searchParams !== null
  )

  const createMutation = useCreateStaff()
  const updateMutation = useUpdateStaff()
  const deleteMutation = useDeleteStaff()

  const isSearchMode = searchParams !== null
  const currentData = isSearchMode ? searchData : listData

  function refreshData() {
    if (isSearchMode) {
      searchData.mutate()
    } else {
      listData.mutate()
    }
  }

  async function handleCreate(data: StaffFormData) {
    try {
      await createMutation.trigger(data)
      toast.success(t("toast.created"))
      refreshData()
    } catch {
      return
    }
  }

  async function handleUpdate(data: StaffFormData) {
    if (!editingStaff) return
    try {
      await updateMutation.trigger({ id: editingStaff.id, data })
      toast.success(t("toast.updated"))
      refreshData()
    } catch {
      return
    }
  }

  async function handleDelete(staffId: string) {
    try {
      await deleteMutation.trigger(staffId)
      toast.success(t("toast.deleted"))
      refreshData()
    } catch {
      return
    }
  }

  function handleEdit(staff: Staff) {
    setEditingStaff(staff)
    setFormDialogOpen(true)
  }

  function handleDeleteClick(staff: Staff) {
    setDeletingStaff(staff)
    setDeleteDialogOpen(true)
  }

  function handleAdd() {
    setEditingStaff(null)
    setFormDialogOpen(true)
  }

  function handleSearch(params: StaffSearchParams) {
    setSearchParams(params)
    setPage(1)
  }

  function handleResetSearch() {
    setSearchParams(null)
    setPage(1)
  }

  function handlePageChange(newPage: number) {
    setPage(newPage)
  }

  function handlePageSizeChange(newSize: number) {
    setPageSize(newSize)
    setPage(1)
  }

  function handleQuickSearch(value: string) {
    if (value.trim()) {
      setSearchParams({ staffId: value.trim(), page: 1 })
    } else {
      setSearchParams(null)
    }
    setPage(1)
  }

  return (
    <div className="mx-auto min-h-svh max-w-6xl p-6">
      <div className="mb-6 flex items-start justify-between">
        <div>
          <h1 className="text-2xl font-semibold">{t("app.title")}</h1>
          <p className="mt-1 text-sm text-muted-foreground">
            {t("app.subtitle")}
          </p>
        </div>
        <div className="flex items-center gap-2">
          <LanguageToggle />
          <Button
            variant="outline"
            size="icon"
            suppressHydrationWarning
            onClick={() =>
              setTheme(resolvedTheme === "dark" ? "light" : "dark")
            }
            title={t("theme.switchTo", {
              mode: resolvedTheme === "dark" ? "light" : "dark",
            })}
          >
            <SunIcon className="size-4 scale-100 rotate-0 transition-transform dark:scale-0 dark:-rotate-90" />
            <MoonIcon className="absolute size-4 scale-0 rotate-90 transition-transform dark:scale-100 dark:rotate-0" />
          </Button>
        </div>
      </div>

      <div className="space-y-4">
        <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
          <div className="flex flex-1 items-center gap-2">
            <div className="relative max-w-xs flex-1">
              <SearchIcon className="absolute top-1/2 left-3 size-4 -translate-y-1/2 text-muted-foreground" />
              <Input
                placeholder={t("search.placeholder")}
                value={quickSearch}
                onChange={(e) => setQuickSearch(e.target.value)}
                onKeyDown={(e) => {
                  if (e.key === "Enter") handleQuickSearch(quickSearch)
                }}
                className="pl-9"
              />
            </div>
            <Button
              variant={showAdvancedSearch ? "secondary" : "outline"}
              size="sm"
              onClick={() => setShowAdvancedSearch((prev) => !prev)}
            >
              <SlidersHorizontalIcon className="mr-2 size-4" />
              {t("search.advanced")}
            </Button>
          </div>

          <div className="flex items-center gap-2">
            <ExportButtons staff={currentData.data} />
            <Button onClick={handleAdd}>
              <PlusIcon className="mr-2 size-4" />
              {t("staff.add")}
            </Button>
          </div>
        </div>

        {showAdvancedSearch && (
          <AdvancedSearchForm
            onSearch={handleSearch}
            onReset={handleResetSearch}
          />
        )}

        {isSearchMode && (
          <div className="flex items-center gap-2 text-sm text-muted-foreground">
            <span>
              {t("search.showingResults")}
              {searchParams.staffId &&
                ` ${t("staff.staffId")}: "${searchParams.staffId}"`}
              {searchParams.fullName &&
                ` ${t("staff.fullName")}: "${searchParams.fullName}"`}
              {searchParams.gender &&
                searchParams.gender !== 0 &&
                ` ${t("staff.gender")}: ${searchParams.gender === 1 ? "Male" : "Female"}`}
            </span>
            <Button
              variant="link"
              size="sm"
              className="h-auto p-0 text-sm"
              onClick={handleResetSearch}
            >
              {t("search.clearFilters")}
            </Button>
          </div>
        )}

        <Separator />

        <StaffTable
          staff={currentData.data}
          isLoading={currentData.isLoading}
          page={currentData.page}
          pageSize={pageSize}
          totalPages={currentData.totalPages}
          totalCount={currentData.totalCount}
          onPageChange={handlePageChange}
          onPageSizeChange={handlePageSizeChange}
          onEdit={handleEdit}
          onDelete={handleDeleteClick}
        />
      </div>

      <StaffFormDialog
        open={formDialogOpen}
        onOpenChange={setFormDialogOpen}
        staff={editingStaff}
        onSubmit={editingStaff ? handleUpdate : handleCreate}
      />

      <StaffDeleteDialog
        open={deleteDialogOpen}
        onOpenChange={setDeleteDialogOpen}
        staff={deletingStaff}
        onConfirm={handleDelete}
      />
    </div>
  )
}
