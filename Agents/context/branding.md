# ECCFIN Branding & Identity

## Church Information
- **English Name:** Ethiopian Evangelical Church in Finland
- **Amharic Name:** የኢትዮጵያ ወንጌላዊት ቤተክርስቲያን በፊንላንድ
- **Website:** https://eecfin.org
- **Credits:** Presented by the **ECCFIN Media Team**
- **Contact:** media@eecfin.org

## Color Palette

### Primary Colors
- **Primary (Deep Navy):** `#1a374f` - Use for main headings, the 'Amen' button background, and logo fallback text
- **Secondary (Earthy Green):** `#6f9078` - Use for borders or secondary icons
- **Accent (Warm Terracotta):** `#d06450` - Use for high-emphasis highlights or the "Amen" click animation
- **Background (Cream):** `#fdfbca` - Use as the main page background for a warm, paper-like feel

### Supporting Colors
- **Muted Elements (Slate/Grey):** `#424555` or `#cad4d7` - Use for the "Media Team" footer and verse reference text
- **Action Highlight (Light Blue):** `#77c4f0` - Use for hover states or link text

## UI Implementation Guidelines

### Logo
- Center `/public/logo.png` at the top
- Max height: 80px
- Display the Amharic name in a clean, elegant font

### Card Style
- **Background:** White or a very subtle tint of `#cad4d7`
- **Border:** 1px solid `#6f9078` (low opacity)
- **Corner Radius:** 28px (Extra rounded for a friendly feel)

### Typography
- **Verse Text:** Use the Primary color (`#1a374f`) with a Serif font
- **Footer:** "Designed with ❤️ by ECCFIN Media Team" in `#424555`
- **Language Support:** Ensure Amharic text renders correctly using a standard Ethiopic-supporting web font (like 'Noto Sans Ethiopic')

### Footer Structure
- Line 1: "© 2026 Ethiopian Evangelical Church in Finland"
- Line 2: "Presented by the Media Team | media@eecfin.org"
- Line 3: Link to eecfin.org

## Animation Guidelines
- Use **Framer Motion** to animate the verse card color from a soft `#fdfbca` glow to a solid state on load
- Animations should be smooth, encouraging, and grounded

## Theme Colors for PWA
- **Theme Color:** `#1a374f` (Primary Deep Navy) - matches existing theme-color meta tag
- **Background Color:** `#fdfbca` (Cream) - matches brand background
- **Display Mode:** `standalone` - removes browser UI for app-like experience
- **Orientation:** `portrait` - optimized for mobile-first QR scanning use case
