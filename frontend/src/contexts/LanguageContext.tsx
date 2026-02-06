import { createContext, useState, type ReactNode } from 'react'
import { translations, type Language, type Translations } from '../i18n/translations'

export interface LanguageContextValue {
  language: Language
  setLanguage: (lang: Language) => void
  t: Translations
}

export const LanguageContext = createContext<LanguageContextValue | null>(null)

function getInitialLanguage(): Language {
  const stored = localStorage.getItem('lang')
  if (stored === 'am' || stored === 'fi') return stored
  return 'en'
}

export function LanguageProvider({ children }: { children: ReactNode }) {
  const [language, setLanguageState] = useState<Language>(getInitialLanguage)

  function setLanguage(lang: Language) {
    setLanguageState(lang)
    localStorage.setItem('lang', lang)
  }

  return (
    <LanguageContext.Provider value={{ language, setLanguage, t: translations[language] }}>
      {children}
    </LanguageContext.Provider>
  )
}
