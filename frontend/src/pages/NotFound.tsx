import { Link } from 'react-router-dom'
import { Home, AlertCircle } from 'lucide-react'
import { useLanguage } from '../hooks/useLanguage'

export default function NotFound() {
  const { t } = useLanguage()

  return (
    <div className="min-h-dvh flex flex-col items-center justify-center px-6 py-12 bg-eccfin-cream font-sans">
      <div className="bg-white/80 rounded-[28px] shadow-xl border border-eccfin-green/20 p-8 max-w-sm w-full flex flex-col items-center gap-6">
        <div className="w-16 h-16 rounded-full bg-eccfin-terracotta/10 flex items-center justify-center">
          <AlertCircle className="w-8 h-8 text-eccfin-terracotta" />
        </div>

        <h1 className="text-3xl font-bold text-eccfin-navy">404</h1>
        
        <div className="text-center space-y-2">
          <p className="text-lg font-semibold text-eccfin-navy">
            {t.pageNotFound || 'Page Not Found'}
          </p>
          <p className="text-eccfin-slate text-sm">
            {t.pageNotFoundMessage || 'The page you are looking for does not exist.'}
          </p>
        </div>

        <Link
          to="/"
          className="flex items-center gap-2 px-6 py-3 bg-eccfin-navy text-white rounded-full font-medium hover:bg-eccfin-navy/90 transition-colors"
        >
          <Home className="w-4 h-4" />
          {t.goHome || 'Go Home'}
        </Link>
      </div>

      <p className="mt-8 text-xs text-eccfin-slate">{t.footerCredit}</p>
    </div>
  )
}

