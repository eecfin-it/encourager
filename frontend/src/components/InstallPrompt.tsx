import { useState, useEffect } from 'react'
import { useLanguage } from '../hooks/useLanguage'

interface BeforeInstallPromptEvent extends Event {
  prompt: () => Promise<void>
  userChoice: Promise<{ outcome: 'accepted' | 'dismissed' }>
}

export function InstallPrompt() {
  const [deferredPrompt, setDeferredPrompt] = useState<BeforeInstallPromptEvent | null>(null)
  const [showPrompt, setShowPrompt] = useState(false)
  const { t } = useLanguage()

  useEffect(() => {
    const handler = (e: Event) => {
      e.preventDefault()
      setDeferredPrompt(e as BeforeInstallPromptEvent)
      setShowPrompt(true)
    }

    window.addEventListener('beforeinstallprompt', handler)

    return () => {
      window.removeEventListener('beforeinstallprompt', handler)
    }
  }, [])

  const handleInstall = async () => {
    if (!deferredPrompt) return

    await deferredPrompt.prompt()
    const { outcome } = await deferredPrompt.userChoice

    if (outcome === 'accepted') {
      setShowPrompt(false)
    }

    setDeferredPrompt(null)
  }

  if (!showPrompt) return null

  return (
    <div className="fixed bottom-4 left-4 right-4 bg-white p-4 rounded-lg shadow-lg border border-eccfin-green z-50">
      <p className="text-eccfin-navy mb-2">{t.installPrompt}</p>
      <div className="flex gap-2">
        <button
          onClick={handleInstall}
          className="px-4 py-2 bg-eccfin-navy text-white rounded-lg"
        >
          {t.install}
        </button>
        <button
          onClick={() => setShowPrompt(false)}
          className="px-4 py-2 text-eccfin-slate rounded-lg"
        >
          {t.notNow}
        </button>
      </div>
    </div>
  )
}
