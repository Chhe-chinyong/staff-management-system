import { z } from "zod"

export const staffFormSchema = z.object({
  id: z
    .string()
    .min(1, "validation.staffIdRequired")
    .length(8, "validation.staffIdLength"),
  fullName: z
    .string()
    .min(1, "validation.fullNameRequired")
    .max(100, "validation.fullNameMax"),
  birthday: z.string().min(1, "validation.birthdayRequired"),
  gender: z.coerce
    .number()
    .int()
    .min(1, "validation.genderRequired")
    .max(2, "validation.invalidGender"),
})

export type StaffFormValues = z.infer<typeof staffFormSchema>

export const searchFormSchema = z.object({
  staffId: z.string().optional(),
  gender: z.coerce.number().int().optional(),
  birthdayFrom: z.string().optional(),
  birthdayTo: z.string().optional(),
})

export type SearchFormValues = z.infer<typeof searchFormSchema>
