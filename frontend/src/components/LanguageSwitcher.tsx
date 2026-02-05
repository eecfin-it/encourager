import { Languages } from 'lucide-react'
import { useLanguage } from '../contexts/LanguageContext'

const langOrder = ['en', 'am', 'fi'] as const
const langLabels: Record<string, string> = { en: 'EN', am: 'አማ', fi: 'FI' }

export default function LanguageSwitcher() {
  const { language, setLanguage } = useLanguage()

  function nextLanguage() {
    const idx = langOrder.indexOf(language)
    setLanguage(langOrder[(idx + 1) % langOrder.length])
  }

  return (
    <button
      onClick={nextLanguage}
      className="
        fixed top-4 right-4 z-50
        flex items-center gap-1.5 px-3 py-1.5
        bg-white/80 backdrop-blur border border-eccfin-green/30 rounded-full
        text-sm font-medium text-eccfin-navy
        hover:bg-white hover:shadow-md
        transition-all duration-200
        cursor-pointer
      "
      aria-label="Switch language"
    >
      <Languages className="w-4 h-4" />
      {langLabels[language]}
    </button>
  )
}
