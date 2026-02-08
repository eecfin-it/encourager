import type { Verse } from '../types/verse'

export class ApiError extends Error {
  readonly status: number

  constructor(message: string, status: number) {
    super(message)
    this.name = 'ApiError'
    this.status = status
  }
}

export async function fetchVerse(language: string, verseId?: number): Promise<Verse> {
  const params = new URLSearchParams({ lang: language })
  if (verseId !== undefined) {
    params.set('verseId', String(verseId))
  }

  const response = await fetch(`/api/verse/random?${params}`)
  if (!response.ok) {
    throw new ApiError(`Failed to fetch verse: ${response.statusText}`, response.status)
  }

  return response.json() as Promise<Verse>
}
