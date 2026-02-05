import { motion } from 'framer-motion'
import { Heart } from 'lucide-react'
import { useLanguage } from '../contexts/LanguageContext'

interface SuccessViewProps {
  onNext: () => void
}

export default function SuccessView({ onNext }: SuccessViewProps) {
  const { t } = useLanguage()

  return (
    <motion.div
      initial={{ opacity: 0, scale: 0.9 }}
      animate={{ opacity: 1, scale: 1 }}
      exit={{ opacity: 0 }}
      transition={{ duration: 0.5 }}
      className="flex flex-col items-center gap-6 max-w-sm w-full text-center"
    >
      <motion.div
        animate={{ scale: [1, 1.2, 1] }}
        transition={{ duration: 1.5, repeat: Infinity }}
      >
        <Heart className="w-16 h-16 text-pink-400 fill-pink-400" />
      </motion.div>

      <h2 className="text-3xl font-bold text-slate-700">{t.godBless}</h2>
      <p className="text-slate-500">{t.mayThisWord}</p>

      <button
        onClick={onNext}
        className="
          mt-4 px-8 py-3 border-2 border-blue-400 text-blue-600
          font-semibold rounded-full cursor-pointer
          hover:bg-blue-50 active:scale-95
          transition-all duration-200
        "
      >
        {t.newBlessing}
      </button>
    </motion.div>
  )
}
