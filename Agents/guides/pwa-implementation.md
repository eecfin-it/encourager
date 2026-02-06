# PWA Implementation Guide

## Goal
Transform the Amen QR Blessing app into a Progressive Web App (PWA) that can be installed and run as a standalone app, similar to Google Meet. Users should be able to "Add to Home Screen" on mobile devices and install it on desktop browsers.

## Implementation Steps

### 1. Install Required Dependencies

```bash
npm install -D vite-plugin-pwa
```

### 2. Configure Vite Plugin

Update `vite.config.ts` to include the PWA plugin. See the full configuration in the original `pwa-implementation.md` file for complete setup.

### 3. Create PWA Icon Assets

Generate app icons in the `public` folder:
- `pwa-192x192.png` - 192x192 pixels (for Android home screen)
- `pwa-512x512.png` - 512x512 pixels (for splash screens and high-res displays)

**Icon Requirements:**
- Use the ECCFIN logo or create an app icon based on the branding
- Ensure icons are square with appropriate padding
- The maskable icon (512x512) should have safe zone padding (20% margin) for Android adaptive icons
- Use brand colors: Primary `#1a374f` or Accent `#d06450`

### 4. Update index.html with PWA Meta Tags

Add PWA-specific meta tags including:
- Theme color: `#1a374f`
- Apple mobile web app capable
- Apple touch icons
- Description and mobile web app settings

### 5. Register Service Worker

The `vite-plugin-pwa` handles service worker registration automatically. Optional custom logic can be added in `main.tsx`.

### 6. Install Prompt Component (Optional)

Create a component to prompt users to install the PWA. See the original `pwa-implementation.md` for implementation details.

## Testing Checklist

- [ ] Build the app: `npm run build`
- [ ] Test in production mode: `npm run preview`
- [ ] HTTPS required: PWA features only work over HTTPS (or localhost)
- [ ] Mobile testing: Add to Home Screen, standalone mode, offline functionality
- [ ] Desktop testing: Install button, standalone window, offline mode
- [ ] Service Worker: Verify registration and offline mode
- [ ] Manifest: Verify icons load and theme colors match branding

## Deployment Considerations

- **HTTPS Required:** PWA features require HTTPS in production (except localhost)
- **Service Worker Scope:** Ensure service worker is served from root
- **Cache Strategy:** API calls use NetworkFirst, static assets are cached
- **Update Strategy:** `autoUpdate` ensures users get latest version automatically

## Branding Alignment

- **Theme Color:** `#1a374f` (Primary Deep Navy)
- **Background Color:** `#fdfbca` (Cream)
- **Display Mode:** `standalone` - removes browser UI for app-like experience
- **Orientation:** `portrait` - optimized for mobile-first QR scanning use case

## Expected Result

After implementation:
- Users can install the app on their devices
- App opens in standalone mode (no browser UI)
- Works offline with cached content
- Automatic updates when new version is deployed
- Native app-like experience on mobile and desktop

## Time Estimate

- Setup and configuration: 30-45 minutes
- Icon creation: 15-30 minutes (if creating from scratch)
- Testing and refinement: 30-60 minutes
- **Total: 2-4 hours**
