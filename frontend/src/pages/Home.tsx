import { useEffect, useState, useRef } from 'react'
import { AnimatePresence } from 'framer-motion'
import VerseDisplay from '../components/VerseDisplay'
import AmenButton from '../components/AmenButton'
import Celebration from '../components/Celebration'
import SuccessView from '../components/SuccessView'
import ReflectionView from '../components/ReflectionView'
import { InstallPrompt } from '../components/InstallPrompt'
import { useLanguage } from '../hooks/useLanguage'

interface Verse {
  verseId: number
  book: string
  chapter: number
  verseNumber: string
  text: string
  language: string
  translations: Record<string, string>
}

interface BlessingData {
  timestamp: string
  verse: Verse
}

interface LegacyVerse {
  text: string
  reference: string
  index: number
}

interface LegacyBlessingData {
  timestamp: string
  verse: LegacyVerse
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

function migrateLegacyData(raw: string): BlessingData | null {
  try {
    const data: LegacyBlessingData = JSON.parse(raw)
    if (!data.timestamp || !data.verse || !isSameDay(data.timestamp)) return null
    // Check if this is legacy format (has reference/index but no verseId)
    if ('reference' in data.verse && !('verseId' in data.verse)) {
      const legacy = data.verse as LegacyVerse
      return {
        timestamp: data.timestamp,
        verse: {
          verseId: legacy.index + 1,
          book: '',
          chapter: 0,
          verseNumber: '0',
          text: legacy.text,
          language: 'en',
          translations: {}
        }
      }
    }
    return null
  } catch { return null }
}

function getSavedBlessing(): BlessingData | null {
  try {
    const raw = localStorage.getItem(STORAGE_KEY)
    if (!raw) return null
    const data = JSON.parse(raw) as BlessingData
    if (data.timestamp && data.verse && data.verse.verseId && isSameDay(data.timestamp)) return data
    // Try legacy migration
    return migrateLegacyData(raw)
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
  const savedVerseId = useRef<number | null>(null)

  // On mount: check if the user already blessed today
  useEffect(() => {
    const saved = getSavedBlessing()
    if (saved) {
      setVerse(saved.verse)
      savedVerseId.current = saved.verse.verseId
      setLockedToday(true)
      setLoading(false)
      skipInitialFetch.current = true
    }
  }, [])

  // Handle language changes — use cached translations (instant) or fetch if needed
  useEffect(() => {
    // Skip the very first run if we loaded a verse from storage
    if (skipInitialFetch.current) {
      skipInitialFetch.current = false
      return
    }

    // If we have cached translations for this verse, switch instantly (no network call)
    if (verse?.translations && verse.translations[language]) {
      setVerse(prev => prev ? {
        ...prev,
        text: prev.translations[language],
        language
      } : null)
      return
    }

    // If a verse is already displayed but has no translations cache, re-fetch by verseId
    const currentVerseId = savedVerseId.current ?? verse?.verseId
    if (currentVerseId !== undefined && currentVerseId !== null && currentVerseId >= 1) {
      fetch(`/api/verse/random?lang=${language}&verseId=${currentVerseId}`)
        .then(res => res.json())
        .then((data: Verse) => setVerse(data))
        .catch(() => {})
      return
    }

    // Initial load: fetch a random verse
    setLoading(true)
    fetch(`/api/verse/random?lang=${language}`)
      .then(res => res.json())
      .then((data: Verse) => setVerse(data))
      .catch(() => setVerse({
        verseId: 0,
        book: '',
        chapter: 0,
        verseNumber: '0',
        text: t.fallbackVerse,
        language,
        translations: {}
      }))
      .finally(() => setLoading(false))
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [language])

  function handleAmen() {
    if (!verse) return
    try {
      const data: BlessingData = { timestamp: new Date().toISOString(), verse }
      localStorage.setItem(STORAGE_KEY, JSON.stringify(data))
    } catch { /* quota exceeded — still show success, just won't persist */ }
    savedVerseId.current = verse.verseId
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
                  <VerseDisplay
                    text={verse.text}
                    book={verse.book}
                    chapter={verse.chapter}
                    verseNumber={verse.verseNumber}
                  />
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

      <InstallPrompt />
    </div>
  )
}
