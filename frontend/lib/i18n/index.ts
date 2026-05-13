import i18n from "i18next"
import { initReactI18next } from "react-i18next"

import en from "./locales/en.json"
import km from "./locales/km.json"

const STORAGE_KEY = "lang"

const resources = {
  en: { translation: en },
  km: { translation: km },
}

i18n.use(initReactI18next).init({
  resources,
  lng:
    typeof window !== "undefined"
      ? (localStorage.getItem(STORAGE_KEY) ?? "km")
      : "km",
  fallbackLng: "en",
  interpolation: {
    escapeValue: false,
  },
})

i18n.on("languageChanged", (lng) => {
  localStorage.setItem(STORAGE_KEY, lng)
  document.documentElement.lang = lng
})

export default i18n
