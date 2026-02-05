import QRCode from 'react-qr-code'
import { ArrowLeft } from 'lucide-react'
import { Link } from 'react-router-dom'
import { useLanguage } from '../contexts/LanguageContext'

export default function Admin() {
  const appUrl = window.location.origin
  const { t } = useLanguage()

  return (
    <div className="min-h-dvh flex flex-col items-center justify-center px-6 py-12 bg-eccfin-cream font-sans">
      <div className="bg-white/80 rounded-[28px] shadow-xl border border-eccfin-green/20 p-8 max-w-sm w-full flex flex-col items-center gap-6">
        <h1 className="text-2xl font-bold text-eccfin-navy">{t.churchAdmin}</h1>
        <p className="text-eccfin-slate text-center text-sm">
          {t.displayQrCode}
        </p>

        <div className="bg-white p-4 rounded-xl border border-eccfin-muted/30">
          <QRCode value={appUrl} size={220} />
        </div>

        <p className="text-xs text-eccfin-slate/60 break-all text-center">{appUrl}</p>

        <Link
          to="/"
          className="flex items-center gap-1 text-eccfin-blue text-sm font-medium hover:underline"
        >
          <ArrowLeft className="w-4 h-4" />
          {t.backToBlessings}
        </Link>
      </div>

      <p className="mt-8 text-xs text-eccfin-slate">{t.footerCredit}</p>
    </div>
  )
}
