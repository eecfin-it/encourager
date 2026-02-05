import QRCode from 'react-qr-code'
import { ArrowLeft } from 'lucide-react'
import { Link } from 'react-router-dom'
import { useLanguage } from '../contexts/LanguageContext'

export default function Admin() {
  const appUrl = window.location.origin
  const { t } = useLanguage()

  return (
    <div className="min-h-dvh flex flex-col items-center justify-center px-6 py-12 bg-gradient-to-br from-blue-50 via-cream-50 to-sky-100 font-sans">
      <div className="bg-white rounded-2xl shadow-xl p-8 max-w-sm w-full flex flex-col items-center gap-6">
        <h1 className="text-2xl font-bold text-slate-700">{t.churchAdmin}</h1>
        <p className="text-slate-500 text-center text-sm">
          {t.displayQrCode}
        </p>

        <div className="bg-white p-4 rounded-xl border border-slate-100">
          <QRCode value={appUrl} size={220} />
        </div>

        <p className="text-xs text-slate-400 break-all text-center">{appUrl}</p>

        <Link
          to="/"
          className="flex items-center gap-1 text-blue-600 text-sm font-medium hover:underline"
        >
          <ArrowLeft className="w-4 h-4" />
          {t.backToBlessings}
        </Link>
      </div>
    </div>
  )
}
