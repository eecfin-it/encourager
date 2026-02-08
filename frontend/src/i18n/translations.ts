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
  pageNotFound: string
  pageNotFoundMessage: string
  goHome: string
  fetchError: string
  installPrompt: string
  install: string
  notNow: string
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
  footerCredit: 'By EECFIN Media Team',
  pageNotFound: 'Page Not Found',
  pageNotFoundMessage: 'The page you are looking for does not exist.',
  goHome: 'Go Home',
  fetchError: 'Could not load verse. Please check your connection.',
  installPrompt: 'Install Amen app for a better experience',
  install: 'Install',
  notNow: 'Not now',
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
  footerCredit: 'በ EECFIN የሚዲያ ቡድን የተዘጋጀ',
  pageNotFound: 'ገጽ አልተገኘም',
  pageNotFoundMessage: 'የሚፈልጉት ገጽ የለም።',
  goHome: 'ወደ መነሻ ይሂዱ',
  fetchError: 'ጥቅስ መጫን አልተቻለም። እባክዎ ግንኙነትዎን ያረጋግጡ።',
  installPrompt: 'ለተሻለ ተሞክሮ የአሜን መተግበሪያን ይጫኑ',
  install: 'ጫን',
  notNow: 'አሁን አይደለም',
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
  footerCredit: 'EECFINin mediatiimin tuottama',
  pageNotFound: 'Sivua ei löytynyt',
  pageNotFoundMessage: 'Etsimääsi sivua ei ole olemassa.',
  goHome: 'Palaa kotiin',
  fetchError: 'Jakeen lataaminen epäonnistui. Tarkista yhteys.',
  installPrompt: 'Asenna Amen-sovellus parempaa kokemusta varten',
  install: 'Asenna',
  notNow: 'Ei nyt',
}

export const translations: Record<Language, Translations> = { en, am, fi }
