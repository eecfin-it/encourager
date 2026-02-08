import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { BrowserRouter, Routes, Route } from 'react-router-dom'
import { registerSW } from 'virtual:pwa-register'
import './index.css'
import Home from './pages/Home'
import Admin from './pages/Admin'
import NotFound from './pages/NotFound'
import { LanguageProvider } from './contexts/LanguageContext'
import LanguageSwitcher from './components/LanguageSwitcher'
import { ErrorBoundary } from './components/ErrorBoundary'

if ('serviceWorker' in navigator) {
  registerSW({
    onNeedRefresh() {
      console.log('New content available, please refresh')
    },
    onOfflineReady() {
      console.log('App ready to work offline')
    },
  })
}

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <ErrorBoundary>
    <LanguageProvider>
      <BrowserRouter>
        <LanguageSwitcher />
        <Routes>
          <Route path="/" element={<Home />} />
          <Route path="/qr" element={<Admin />} />
          <Route path="*" element={<NotFound />} />
        </Routes>
      </BrowserRouter>
    </LanguageProvider>
    </ErrorBoundary>
  </StrictMode>,
)
