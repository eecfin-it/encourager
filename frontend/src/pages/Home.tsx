import { useEffect, useState, useRef } from 'react'
import { AnimatePresence } from 'framer-motion'
import VerseDisplay from '../components/VerseDisplay'
import AmenButton from '../components/AmenButton'
import Celebration from '../components/Celebration'
import SuccessView from '../components/SuccessView'
import ReflectionView from '../components/ReflectionView'
import { useLanguage } from '../contexts/LanguageContext'

interface Verse {
  text: string
  reference: string
  index: number
}

interface BlessingData {
  timestamp: string
  verse: Verse
}

const STORAGE_KEY = 'last_blessing_data'

function isSameDay(iso: string): boolean {
  const saved = new Date(iso)
  const now = new Date()
  return (
    saved.getFullYear() === now.getFullYear() &&
    saved.getMonth() === now.getMonth() &&
    saved.getDate() === now.getDate()
  )
}

function getSavedBlessing(): BlessingData | null {
  try {
    const raw = localStorage.getItem(STORAGE_KEY)
    if (!raw) return null
    const data: BlessingData = JSON.parse(raw)
    if (data.timestamp && data.verse && isSameDay(data.timestamp)) return data
  } catch { /* ignore corrupt data */ }
  return null
}

export default function Home() {
  const [verse, setVerse] = useState<Verse | null>(null)
  const [accepted, setAccepted] = useState(false)
  const [loading, setLoading] = useState(true)
  const [lockedToday, setLockedToday] = useState(false)
  const { language, t } = useLanguage()
  const skipInitialFetch = useRef(false)
  const savedIndex = useRef<number | null>(null)

  // On mount: check if the user already blessed today
  useEffect(() => {
    const saved = getSavedBlessing()
    if (saved) {
      setVerse(saved.verse)
      savedIndex.current = saved.verse.index
      setLockedToday(true)
      setLoading(false)
      skipInitialFetch.current = true
    }
  }, [])

  // Fetch a verse on mount (if not locked) and whenever language changes
  useEffect(() => {
    // Skip the very first run if we loaded a verse from storage
    if (skipInitialFetch.current) {
      skipInitialFetch.current = false
      return
    }

    // When locked, re-fetch the SAME verse (by index) in the new language
    if (lockedToday && savedIndex.current !== null) {
      fetch(`/api/verse/random?lang=${language}&index=${savedIndex.current}`)
        .then(res => res.json())
        .then((data: Verse) => setVerse(data))
        .catch(() => {})
      return
    }

    // Normal mode: show loading spinner while fetching a random verse
    setLoading(true)
    fetch(`/api/verse/random?lang=${language}`)
      .then(res => res.json())
      .then((data: Verse) => setVerse(data))
      .catch(() => setVerse({ text: t.fallbackVerse, reference: t.fallbackReference, index: -1 }))
      .finally(() => setLoading(false))
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [language])

  function handleAmen() {
    if (!verse) return
    try {
      const data: BlessingData = { timestamp: new Date().toISOString(), verse }
      localStorage.setItem(STORAGE_KEY, JSON.stringify(data))
    } catch { /* quota exceeded — still show success, just won't persist */ }
    savedIndex.current = verse.index
    setLockedToday(true)
    setAccepted(true)
  }

  function handleNext() {
    setAccepted(false)
  }

  // Reflection screen — user already blessed today
  if (lockedToday && !accepted) {
    return verse ? <ReflectionView verse={verse} /> : null
  }

  return (
    <div className="min-h-dvh flex flex-col items-center justify-between px-6 py-10 bg-eccfin-cream font-sans">
      <Celebration fire={accepted} />

      {/* Header: logo + church name */}
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

      {/* Main content */}
      <main className="flex-1 flex flex-col items-center justify-center w-full">
        <AnimatePresence mode="wait">
          {!accepted ? (
            <div
              key="verse"
              className="flex flex-col items-center gap-8 w-full"
            >
              {loading ? (
                <div className="h-48 flex items-center">
                  <div className="w-8 h-8 border-4 border-eccfin-green/30 border-t-eccfin-navy rounded-full animate-spin" />
                </div>
              ) : (
                verse && (
                  <VerseDisplay text={verse.text} reference={verse.reference} />
                )
              )}

              <AmenButton onClick={handleAmen} disabled={loading} />
            </div>
          ) : (
            <SuccessView key="success" onNext={handleNext} />
          )}
        </AnimatePresence>
      </main>

      {/* Footer */}
      <footer className="mt-8 text-center">
        <p className="text-xs text-eccfin-slate">{t.footerCredit}</p>
      </footer>
    </div>
  )
}
