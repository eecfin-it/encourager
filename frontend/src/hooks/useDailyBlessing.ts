import { useRef, useState } from 'react'
import type { BlessingData, LegacyBlessingData, LegacyVerse, Verse } from '../types/verse'

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
          translations: {},
        },
      }
    }
    return null
  } catch {
    return null
  }
}

function getSavedBlessing(): BlessingData | null {
  try {
    const raw = localStorage.getItem(STORAGE_KEY)
    if (!raw) return null
    const data = JSON.parse(raw) as BlessingData
    if (data.timestamp && data.verse && data.verse.verseId && isSameDay(data.timestamp)) return data
    return migrateLegacyData(raw)
  } catch {
    return null
  }
}

export function useDailyBlessing() {
  const [state] = useState(() => {
    const saved = getSavedBlessing()
    return {
      savedVerse: saved?.verse ?? null,
      savedVerseId: saved?.verse.verseId ?? null,
    }
  })
  const [lockedToday, setLockedToday] = useState(() => !!getSavedBlessing())
  const savedVerseId = useRef<number | null>(state.savedVerseId)

  function lockBlessing(verse: Verse) {
    try {
      const data: BlessingData = { timestamp: new Date().toISOString(), verse }
      localStorage.setItem(STORAGE_KEY, JSON.stringify(data))
    } catch {
      /* quota exceeded — still show success, just won't persist */
    }
    savedVerseId.current = verse.verseId
    setLockedToday(true)
  }

  return { savedVerse: state.savedVerse, lockedToday, lockBlessing, savedVerseId }
}
