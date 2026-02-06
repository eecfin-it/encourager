# PWA Implementation Instructions: Standalone App Experience

## Goal
Transform the Amen QR Blessing app into a Progressive Web App (PWA) that can be installed and run as a standalone app, similar to Google Meet. Users should be able to "Add to Home Screen" on mobile devices and install it on desktop browsers.

## Implementation Steps

### 1. Install Required Dependencies

Install `vite-plugin-pwa` which provides automatic service worker generation and PWA configuration:

```bash
npm install -D vite-plugin-pwa
```

### 2. Configure Vite Plugin

Update `vite.config.ts` to include the PWA plugin:

```typescript
import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import tailwindcss from '@tailwindcss/vite'
import { VitePWA } from 'vite-plugin-pwa'

export default defineConfig({
  plugins: [
    react(), 
    tailwindcss(),
    VitePWA({
      registerType: 'autoUpdate',
      includeAssets: ['favicon.png', 'logo.png'],
      manifest: {
        name: 'Amen — ECCFIN Blessing',
        short_name: 'Amen',
        description: 'Receive encouraging verses through QR code blessings',
        theme_color: '#1a374f',
        background_color: '#fdfbca',
        display: 'standalone',
        orientation: 'portrait',
        scope: '/',
        start_url: '/',
        icons: [
          {
            src: 'pwa-192x192.png',
            sizes: '192x192',
            type: 'image/png'
          },
          {
            src: 'pwa-512x512.png',
            sizes: '512x512',
            type: 'image/png'
          },
          {
            src: 'pwa-512x512.png',
            sizes: '512x512',
            type: 'image/png',
            purpose: 'any maskable'
          }
        ]
      },
      workbox: {
        globPatterns: ['**/*.{js,css,html,ico,png,svg,woff2}'],
        runtimeCaching: [
          {
            urlPattern: /^https:\/\/fonts\.googleapis\.com\/.*/i,
            handler: 'CacheFirst',
            options: {
              cacheName: 'google-fonts-cache',
              expiration: {
                maxEntries: 10,
                maxAgeSeconds: 60 * 60 * 24 * 365 // 1 year
              },
              cacheableResponse: {
                statuses: [0, 200]
              }
            }
          },
          {
            urlPattern: /^https:\/\/fonts\.gstatic\.com\/.*/i,
            handler: 'CacheFirst',
            options: {
              cacheName: 'google-fonts-static-cache',
              expiration: {
                maxEntries: 10,
                maxAgeSeconds: 60 * 60 * 24 * 365 // 1 year
              },
              cacheableResponse: {
                statuses: [0, 200]
              }
            }
          },
          {
            urlPattern: /^\/api\/.*/i,
            handler: 'NetworkFirst',
            options: {
              cacheName: 'api-cache',
              expiration: {
                maxEntries: 50,
                maxAgeSeconds: 60 * 5 // 5 minutes
              },
              networkTimeoutSeconds: 10
            }
          }
        ]
      },
      devOptions: {
        enabled: true,
        type: 'module'
      }
    })
  ],
  server: {
    proxy: {
      '/api': 'http://localhost:5226',
    },
  },
})
```

### 3. Create PWA Icon Assets

Generate app icons in the `public` folder:
- `pwa-192x192.png` - 192x192 pixels (for Android home screen)
- `pwa-512x512.png` - 512x512 pixels (for splash screens and high-res displays)

**Icon Requirements:**
- Use the ECCFIN logo or create an app icon based on the branding
- Ensure icons are square with appropriate padding
- The maskable icon (512x512) should have safe zone padding (20% margin) for Android adaptive icons
- Use brand colors: Primary `#1a374f` or Accent `#d06450`

**Note:** If logo.png exists, you can use it as a base. Otherwise, create simple icon designs using the brand colors.

### 4. Update index.html with PWA Meta Tags

Add PWA-specific meta tags to `index.html`:

```html
<!doctype html>
<html lang="en">
  <head>
    <meta charset="UTF-8" />
    <link rel="icon" type="image/png" href="/favicon.png" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <meta name="theme-color" content="#1a374f" />
    
    <!-- PWA Meta Tags -->
    <meta name="description" content="Receive encouraging verses through QR code blessings" />
    <meta name="mobile-web-app-capable" content="yes" />
    <meta name="apple-mobile-web-app-capable" content="yes" />
    <meta name="apple-mobile-web-app-status-bar-style" content="black-translucent" />
    <meta name="apple-mobile-web-app-title" content="Amen" />
    
    <!-- Apple Touch Icons -->
    <link rel="apple-touch-icon" href="/pwa-192x192.png" />
    
    <title>Amen — ECCFIN Blessing</title>
    <link rel="preconnect" href="https://fonts.googleapis.com" />
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin />
    <link href="https://fonts.googleapis.com/css2?family=Merriweather:ital,wght@0,400;0,700;1,400&display=swap" rel="stylesheet" />
  </head>
  <body>
    <div id="root"></div>
    <script type="module" src="/src/main.tsx"></script>
  </head>
</html>
```

### 5. Register Service Worker in Main Entry Point

Update `src/main.tsx` to register the service worker (optional - vite-plugin-pwa handles this automatically, but you can add custom logic):

```typescript
import { registerSW } from 'virtual:pwa-register'

if ('serviceWorker' in navigator) {
  const updateSW = registerSW({
    onNeedRefresh() {
      // Show update notification to user
      console.log('New content available, please refresh')
    },
    onOfflineReady() {
      console.log('App ready to work offline')
    },
  })
}
```

### 6. Create Install Prompt Component (Optional Enhancement)

Create a component to prompt users to install the PWA:

```typescript
// src/components/InstallPrompt.tsx
import { useState, useEffect } from 'react'

interface BeforeInstallPromptEvent extends Event {
  prompt: () => Promise<void>
  userChoice: Promise<{ outcome: 'accepted' | 'dismissed' }>
}

export function InstallPrompt() {
  const [deferredPrompt, setDeferredPrompt] = useState<BeforeInstallPromptEvent | null>(null)
  const [showPrompt, setShowPrompt] = useState(false)

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
    <div className="fixed bottom-4 left-4 right-4 bg-white p-4 rounded-lg shadow-lg border border-[#6f9078] z-50">
      <p className="text-[#1a374f] mb-2">Install Amen app for a better experience</p>
      <div className="flex gap-2">
        <button
          onClick={handleInstall}
          className="px-4 py-2 bg-[#1a374f] text-white rounded-lg"
        >
          Install
        </button>
        <button
          onClick={() => setShowPrompt(false)}
          className="px-4 py-2 text-[#424555] rounded-lg"
        >
          Not now
        </button>
      </div>
    </div>
  )
}
```

### 7. Testing Checklist

After implementation, test the following:

- [ ] **Build the app**: `npm run build`
- [ ] **Test in production mode**: `npm run preview` (or serve dist folder)
- [ ] **HTTPS required**: PWA features only work over HTTPS (or localhost)
- [ ] **Mobile testing**: 
  - Open on mobile browser (Chrome/Safari)
  - Check "Add to Home Screen" option appears
  - Install and verify standalone mode (no browser UI)
  - Test offline functionality
- [ ] **Desktop testing**:
  - Chrome/Edge: Check install button in address bar
  - Verify app opens in standalone window
  - Test offline mode
- [ ] **Service Worker**: 
  - Check DevTools > Application > Service Workers
  - Verify service worker is registered
  - Test offline mode by disabling network
- [ ] **Manifest**: 
  - Check DevTools > Application > Manifest
  - Verify all icons load correctly
  - Check theme colors match branding

### 8. Deployment Considerations

- **HTTPS Required**: PWA features require HTTPS in production (except localhost)
- **Service Worker Scope**: Ensure service worker is served from root or configure scope correctly
- **Cache Strategy**: API calls use NetworkFirst to ensure fresh data, static assets are cached
- **Update Strategy**: `autoUpdate` ensures users get latest version automatically

### 9. Branding Alignment

- **Theme Color**: `#1a374f` (Primary Deep Navy) - matches existing theme-color meta tag
- **Background Color**: `#fdfbca` (Cream) - matches brand background
- **Display Mode**: `standalone` - removes browser UI for app-like experience
- **Orientation**: `portrait` - optimized for mobile-first QR scanning use case

## Expected Result

After implementation:
- Users can install the app on their devices
- App opens in standalone mode (no browser UI) like Google Meet
- Works offline with cached content
- Automatic updates when new version is deployed
- Native app-like experience on mobile and desktop

## Time Estimate

- Setup and configuration: 30-45 minutes
- Icon creation: 15-30 minutes (if creating from scratch)
- Testing and refinement: 30-60 minutes
- **Total: 2-4 hours**
