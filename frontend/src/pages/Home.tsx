import { useState } from 'react'
import { AnimatePresence } from 'framer-motion'
import VerseDisplay from '../components/VerseDisplay'
import AmenButton from '../components/AmenButton'
import Celebration from '../components/Celebration'
import SuccessView from '../components/SuccessView'
import ReflectionView from '../components/ReflectionView'
import { InstallPrompt } from '../components/InstallPrompt'
import { useLanguage } from '../hooks/useLanguage'
import { useDailyBlessing } from '../hooks/useDailyBlessing'
import { useVerse } from '../hooks/useVerse'

export default function Home() {
  const [accepted, setAccepted] = useState(false)
  const { t } = useLanguage()
  const { savedVerse, lockedToday, lockBlessing, savedVerseId } = useDailyBlessing()
  const { verse, loading, error } = useVerse({ initialVerse: savedVerse, savedVerseId })

  function handleAmen() {
    if (!verse) return
    lockBlessing(verse)
    setAccepted(true)
  }

  function handleNext() {
    setAccepted(false)
  }

  if (lockedToday && !accepted) {
    return verse ? <ReflectionView verse={verse} /> : null
  }

  return (
    <div className="min-h-dvh flex flex-col items-center justify-between px-6 py-10 bg-eccfin-cream font-sans">
      <Celebration fire={accepted} />

      <header className="flex flex-col items-center gap-3 mb-6">
        <img
          src="/logo-sm.png"
          alt="ECCFIN"
          className="h-20 w-auto object-contain"
          onError={(e) => {
            e.currentTarget.style.display = 'none'
          }}
        />
        <h1 className="text-sm font-bold text-eccfin-navy tracking-wide text-center">
          {t.churchName}
        </h1>
      </header>

      <main className="flex-1 flex flex-col items-center justify-center w-full">
        <AnimatePresence mode="wait">
          {!accepted ? (
            <div key="verse" className="flex flex-col items-center gap-8 w-full">
              {loading ? (
                <div className="h-48 flex items-center">
                  <div className="w-8 h-8 border-4 border-eccfin-green/30 border-t-eccfin-navy rounded-full animate-spin" />
                </div>
              ) : (
                <>
                  {error && (
                    <p className="text-eccfin-terracotta text-sm text-center">{error}</p>
                  )}
                  {verse && (
                    <VerseDisplay
                      text={verse.text}
                      book={verse.book}
                      chapter={verse.chapter}
                      verseNumber={verse.verseNumber}
                    />
                  )}
                </>
              )}

              <AmenButton onClick={handleAmen} disabled={loading} />
            </div>
          ) : (
            <SuccessView key="success" onNext={handleNext} />
          )}
        </AnimatePresence>
      </main>

      <footer className="mt-8 text-center">
        <p className="text-xs text-eccfin-slate">{t.footerCredit}</p>
      </footer>

      <InstallPrompt />
    </div>
  )
}
