import { Heart } from 'lucide-react'
import { useLanguage } from '../contexts/LanguageContext'

interface AmenButtonProps {
  onClick: () => void
  disabled?: boolean
}

export default function AmenButton({ onClick, disabled }: AmenButtonProps) {
  const { t } = useLanguage()

  return (
    <button
      onClick={onClick}
      disabled={disabled}
      className="
        relative px-14 py-4 text-xl font-bold text-white rounded-full cursor-pointer
        bg-gradient-to-r from-blue-500 to-indigo-500
        shadow-[0_0_24px_rgba(99,102,241,0.35)]
        hover:shadow-[0_0_36px_rgba(99,102,241,0.5)] hover:scale-105
        active:scale-95
        transition-all duration-200
        disabled:opacity-50 disabled:cursor-not-allowed disabled:shadow-none
      "
    >
      <span className="flex items-center gap-2">
        <Heart className="w-5 h-5" />
        {t.amen}
      </span>
    </button>
  )
}
