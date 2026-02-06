# Frontend Setup Guide

## Overview
The frontend is a React 19 application built with Vite, TypeScript, and Tailwind CSS.

## Initial Setup

### Prerequisites
- Node.js 22+
- npm or yarn

### Project Structure
```
frontend/
├── src/
│   ├── components/        # React components
│   │   ├── VerseDisplay.tsx
│   │   ├── AmenButton.tsx
│   │   ├── SuccessView.tsx
│   │   └── ...
│   ├── pages/            # Page components
│   │   ├── Home.tsx
│   │   └── Admin.tsx
│   ├── contexts/         # React contexts
│   │   └── LanguageContext.tsx
│   ├── hooks/           # Custom hooks
│   │   └── useLanguage.ts
│   ├── i18n/            # Translations
│   │   └── translations.ts
│   └── main.tsx         # Entry point
├── public/              # Static assets
└── vite.config.ts       # Vite configuration
```

## Key Components

### VerseDisplay
- Displays verse text and reference
- Uses Framer Motion for fade-in animation
- Responsive card design

### AmenButton
- Large, prominent button
- Triggers confetti effect on click
- Disabled state during loading

### SuccessView
- Shows success message after "Amen" click
- Provides "Next QR" option
- Celebration animation

## State Management

### Local State
- Use `useState` for component-specific state
- Track verse data, acceptance status, loading states

### Context API
- `LanguageContext` for language selection
- Provides language switching functionality
- Persists language preference

## Styling

### Tailwind CSS
- Mobile-first responsive design
- Use utility classes
- Reference branding colors from context/branding.md

### Animations
- Framer Motion for smooth transitions
- Confetti effect using canvas-confetti
- Fade-in animations for verse cards

## Local Development

### Running Locally
```bash
cd frontend
npm install
npm run dev
```

Frontend will be available at `http://localhost:5173`

### Building
```bash
npm run build
```

### Linting
```bash
npm run lint
```

## API Integration

### Fetching Verses
- Use native Fetch API
- Proxy configured in Vite for `/api/*` routes
- Handles loading and error states

### Language Support
- Supports English, Amharic, and Finnish
- Language selector in UI
- Persists selection in localStorage

## PWA Configuration
See guides/pwa-implementation.md for PWA setup details.
