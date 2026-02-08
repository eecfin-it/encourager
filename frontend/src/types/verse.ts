export interface Verse {
  verseId: number
  book: string
  chapter: number
  verseNumber: string
  text: string
  language: string
  translations: Record<string, string>
}

export interface BlessingData {
  timestamp: string
  verse: Verse
}

export interface LegacyVerse {
  text: string
  reference: string
  index: number
}

export interface LegacyBlessingData {
  timestamp: string
  verse: LegacyVerse
}
