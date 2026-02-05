import { useEffect, useState, useCallback } from 'react'
import { AnimatePresence } from 'framer-motion'
import VerseDisplay from '../components/VerseDisplay'
import AmenButton from '../components/AmenButton'
import Celebration from '../components/Celebration'
import SuccessView from '../components/SuccessView'
import { useLanguage } from '../contexts/LanguageContext'

interface Verse {
  text: string
  reference: string
}

export default function Home() {
  const [verse, setVerse] = useState<Verse | null>(null)
  const [accepted, setAccepted] = useState(false)
  const [loading, setLoading] = useState(true)
  const { language, t } = useLanguage()

  const fetchVerse = useCallback(async () => {
    setLoading(true)
    setAccepted(false)
    try {
      const res = await fetch(`/api/verse/random?lang=${language}`)
      const data: Verse = await res.json()
      setVerse(data)
    } catch {
      setVerse({
        text: t.fallbackVerse,
        reference: t.fallbackReference,
      })
    } finally {
      setLoading(false)
    }
  }, [language, t.fallbackVerse, t.fallbackReference])

  useEffect(() => {
    fetchVerse()
  }, [fetchVerse])

  return (
    <div className="min-h-dvh flex flex-col items-center justify-center px-6 py-12 bg-gradient-to-br from-slate-50 to-blue-50 font-sans">
      <Celebration fire={accepted} />

      <AnimatePresence mode="wait">
        {!accepted ? (
          <div
            key="verse"
            className="flex flex-col items-center gap-8 w-full"
          >
            {loading ? (
              <div className="h-48 flex items-center">
                <div className="w-8 h-8 border-4 border-blue-200 border-t-blue-500 rounded-full animate-spin" />
              </div>
            ) : (
              verse && (
                <VerseDisplay text={verse.text} reference={verse.reference} />
              )
            )}

            <AmenButton onClick={() => setAccepted(true)} disabled={loading} />
          </div>
        ) : (
          <SuccessView key="success" onNext={fetchVerse} />
        )}
      </AnimatePresence>
    </div>
  )
}
