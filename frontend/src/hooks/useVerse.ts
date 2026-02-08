import { useEffect, useRef, useState } from 'react'
import type { Verse } from '../types/verse'
import { fetchVerse } from '../services/api'
import { useLanguage } from './useLanguage'

interface UseVerseOptions {
  initialVerse: Verse | null
  savedVerseId: React.RefObject<number | null>
}

export function useVerse({ initialVerse, savedVerseId }: UseVerseOptions) {
  const [verse, setVerse] = useState<Verse | null>(initialVerse)
  const [loading, setLoading] = useState(!initialVerse)
  const [error, setError] = useState<string | null>(null)
  const { language, t } = useLanguage()
  const skipInitialFetch = useRef(!!initialVerse)

  useEffect(() => {
    if (skipInitialFetch.current) {
      skipInitialFetch.current = false
      return
    }

    if (verse?.translations && verse.translations[language]) {
      setVerse((prev) =>
        prev
          ? {
              ...prev,
              text: prev.translations[language],
              language,
            }
          : null,
      )
      return
    }

    const currentVerseId = savedVerseId.current ?? verse?.verseId
    if (currentVerseId !== undefined && currentVerseId !== null && currentVerseId >= 1) {
      setError(null)
      fetchVerse(language, currentVerseId)
        .then((data) => setVerse(data))
        .catch(() => setError(t.fetchError))
      return
    }

    setLoading(true)
    setError(null)
    fetchVerse(language)
      .then((data) => setVerse(data))
      .catch(() => {
        setVerse({
          verseId: 0,
          book: '',
          chapter: 0,
          verseNumber: '0',
          text: t.fallbackVerse,
          language,
          translations: {},
        })
        setError(t.fetchError)
      })
      .finally(() => setLoading(false))
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [language])

  return { verse, loading, error, setVerse }
}
