export type Language = 'en' | 'am'

export interface Translations {
  amen: string
  godBless: string
  mayThisWord: string
  newBlessing: string
  churchAdmin: string
  displayQrCode: string
  backToBlessings: string
  fallbackVerse: string
  fallbackReference: string
}

const en: Translations = {
  amen: 'Amen',
  godBless: 'God Bless',
  mayThisWord: 'May this word stay with you today.',
  newBlessing: 'New Blessing',
  churchAdmin: 'Church Admin',
  displayQrCode: 'Display this QR code for your congregation to receive a daily blessing.',
  backToBlessings: 'Back to Blessings',
  fallbackVerse: 'The Lord is good to all; he has compassion on all he has made.',
  fallbackReference: 'Psalm 145:9',
}

const am: Translations = {
  amen: 'አሜን',
  godBless: 'እግዚአብሔር ይባርክህ',
  mayThisWord: 'ይህ ቃል ዛሬ ከአንተ ጋር ይሁን።',
  newBlessing: 'አዲስ በረከት',
  churchAdmin: 'የቤተ ክርስቲያን አስተዳዳሪ',
  displayQrCode: 'ምእመናን የዕለት በረከት እንዲቀበሉ ይህን QR ኮድ አሳዩ።',
  backToBlessings: 'ወደ በረከቶች ተመለስ',
  fallbackVerse: 'እግዚአብሔር ለሁሉ ቸር ነው፤ ለፍጥረቱም ሁሉ ይራራል።',
  fallbackReference: 'መዝሙር 145፥9',
}

export const translations: Record<Language, Translations> = { en, am }
