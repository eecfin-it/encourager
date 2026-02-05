import { Languages } from 'lucide-react'
import { useLanguage } from '../contexts/LanguageContext'

export default function LanguageSwitcher() {
  const { language, setLanguage } = useLanguage()

  return (
    <button
      onClick={() => setLanguage(language === 'en' ? 'am' : 'en')}
      className="
        fixed top-4 right-4 z-50
        flex items-center gap-1.5 px-3 py-1.5
        bg-white/80 backdrop-blur border border-slate-200 rounded-full
        text-sm font-medium text-slate-600
        hover:bg-white hover:shadow-md
        transition-all duration-200
        cursor-pointer
      "
      aria-label="Switch language"
    >
      <Languages className="w-4 h-4" />
      {language === 'en' ? 'አማ' : 'EN'}
    </button>
  )
}
