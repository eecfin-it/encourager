export type Language = 'en' | 'am' | 'fi'

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
  alreadyReceived: string
  nextBlessingAt: string
  churchName: string
  footerCredit: string
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
  alreadyReceived: 'You have already received your Word for today. May it stay in your heart.',
  nextBlessingAt: 'Next blessing available at midnight.',
  churchName: 'Ethiopian Evangelical Church in Finland',
  footerCredit: 'Designed with ❤️ by ECCFIN Media Team',
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
  alreadyReceived: 'የዛሬውን ቃል አስቀድመው ተቀብለዋል። በልብዎ ይኑር።',
  nextBlessingAt: 'ቀጣይ በረከት ከእኩለ ሌሊት በኋላ ይገኛል።',
  churchName: 'የኢትዮጵያ ወንጌላዊት ቤተክርስቲያን በፊንላንድ',
  footerCredit: 'በ ❤️ የተሰራ በ ECCFIN ሚዲያ ቡድን',
}

const fi: Translations = {
  amen: 'Aamen',
  godBless: 'Jumala siunatkoon',
  mayThisWord: 'Tämä sana kulkekoon kanssasi tänään.',
  newBlessing: 'Uusi siunaus',
  churchAdmin: 'Seurakunnan hallinta',
  displayQrCode: 'Näytä tämä QR-koodi seurakunnallesi päivittäisen siunauksen vastaanottamiseksi.',
  backToBlessings: 'Takaisin siunauksiin',
  fallbackVerse: 'Herra on hyvä kaikille, hän armahtaa kaikkia luotujaan.',
  fallbackReference: 'Psalmi 145:9',
  alreadyReceived: 'Olet jo saanut sanasi tälle päivälle. Säilytä se sydämessäsi.',
  nextBlessingAt: 'Seuraava siunaus on saatavilla keskiyöllä.',
  churchName: 'Etiopian evankelinen kirkko Suomessa',
  footerCredit: 'Suunniteltu ❤️:lla — ECCFIN Media Team',
}

export const translations: Record<Language, Translations> = { en, am, fi }
