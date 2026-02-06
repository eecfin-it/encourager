# Project Overview: Amen QR Blessing App

## Goal
A mobile-first web app for church members to receive a random encouraging verse after scanning a QR code. The app encourages daily reflection by limiting users to one blessing per day.

## Tech Stack

### Backend
- **Framework:** .NET 10 Web API (Minimal APIs)
- **Language:** C# 14
- **Latest Features:** Use C# 14 features and modern .NET patterns

### Frontend
- **Framework:** React 19 with TypeScript
- **Build Tool:** Vite
- **Styling:** Tailwind CSS
- **Icons:** Lucide React
- **Animations:** Framer Motion
- **Confetti:** canvas-confetti
- **Communication:** Fetch API (native)

### Latest Tech Features
- Use React Compiler features (no need for manual useMemo/useCallback)
- Leverage C# 14 features
- Modern ES modules and TypeScript 5.9+

## Core Features

1. **Verse API:** Backend endpoint `/api/verse/random` that returns a JSON object with verse text and reference
2. **Multi-language Support:** English, Amharic (አማርኛ), and Finnish
3. **Mobile-First UI:** Clean, spiritual, and high-end mobile interface
4. **Daily Limit:** One blessing per day to encourage reflection
5. **QR Generation:** Admin route (`/admin`) that displays a permanent QR code pointing to the home page
6. **PWA Support:** Progressive Web App that can be installed on devices

## Primary User Flow

1. Scan QR code (or visit directly)
2. See verse displayed with fade-in animation
3. Click "Amen" button
4. Trigger confetti effect
5. Show success message with "Next QR" option
6. If already received blessing today, show reflection screen with countdown

## Design Vibe

- **Tone:** Encouraging, grounded, and modern
- **Visual Style:** Soft gradients (light blues/creams)
- **Animations:** Smooth verse transitions using Framer Motion
- **Color Palette:** Warm, spiritual colors (see branding.md)

## Architecture

- **Frontend:** React SPA served from S3 via CloudFront
- **Backend:** .NET 10 Lambda function via API Gateway
- **Infrastructure:** AWS SAM (CloudFormation)
- **Deployment:** GitHub Actions CI/CD
