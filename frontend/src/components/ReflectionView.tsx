import { useEffect, useState } from 'react'
import { motion } from 'framer-motion'
import { Moon } from 'lucide-react'
import VerseDisplay from './VerseDisplay'
import { useLanguage } from '../hooks/useLanguage'

interface ReflectionViewProps {
  verse: { text: string; book: string; chapter: number; verseNumber: string }
}

function getTimeUntilMidnight() {
  const now = new Date()
  const midnight = new Date(now)
  midnight.setHours(24, 0, 0, 0)
  return midnight.getTime() - now.getTime()
}

function formatCountdown(ms: number) {
  const totalSeconds = Math.max(0, Math.floor(ms / 1000))
  const h = Math.floor(totalSeconds / 3600)
  const m = Math.floor((totalSeconds % 3600) / 60)
  const s = totalSeconds % 60
  return `${String(h).padStart(2, '0')}:${String(m).padStart(2, '0')}:${String(s).padStart(2, '0')}`
}

export default function ReflectionView({ verse }: ReflectionViewProps) {
  const { t } = useLanguage()
  const [remaining, setRemaining] = useState(getTimeUntilMidnight)

  useEffect(() => {
    const id = setInterval(() => setRemaining(getTimeUntilMidnight()), 1000)
    return () => clearInterval(id)
  }, [])

  return (
    <motion.div
      initial={{ opacity: 0 }}
      animate={{ opacity: 1 }}
      transition={{ duration: 0.6 }}
      className="min-h-dvh flex flex-col items-center justify-center px-6 py-12 bg-gradient-to-br from-eccfin-navy via-eccfin-slate to-eccfin-navy font-sans"
    >
      <div className="flex flex-col items-center gap-8 max-w-md w-full text-center">
        <Moon className="w-12 h-12 text-eccfin-blue" />

        <p className="text-eccfin-muted text-lg leading-relaxed">
          {t.alreadyReceived}
        </p>

        <div className="w-full">
          <VerseDisplay
            text={verse.text}
            book={verse.book}
            chapter={verse.chapter}
            verseNumber={verse.verseNumber}
          />
        </div>

        <div className="flex flex-col items-center gap-2 mt-4">
          <p className="text-eccfin-muted/70 text-sm">{t.nextBlessingAt}</p>
          <p className="text-3xl font-mono font-bold text-eccfin-blue tracking-widest">
            {formatCountdown(remaining)}
          </p>
        </div>

        <p className="text-eccfin-muted/50 text-xs mt-8">{t.footerCredit}</p>
      </div>
    </motion.div>
  )
}
