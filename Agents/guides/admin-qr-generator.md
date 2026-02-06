# Admin QR Generator Guide

## Overview
Create an admin route for generating QR codes that can be projected on church screens.

## Implementation

### Route
- **Path:** `/admin`
- **Access:** NOT linked in the main UI (direct URL access only)
- **Purpose:** Display QR code for church projection

### Requirements

#### Library
- Use `qrcode.react` library for QR code generation
- Install: `npm install qrcode.react`

#### Display
- Large, high-quality QR code
- Points to the site's root URL (production URL)
- Optimized for projection on screens
- High contrast for visibility

### Implementation Example

```typescript
import { QRCodeSVG } from 'qrcode.react';

export default function Admin() {
  const siteUrl = import.meta.env.PROD 
    ? 'https://your-production-url.com' 
    : 'http://localhost:5173';

  return (
    <div className="flex flex-col items-center justify-center min-h-screen p-8">
      <h1 className="text-2xl mb-8">Admin QR Code</h1>
      <QRCodeSVG
        value={siteUrl}
        size={512}
        level="H"
        includeMargin={true}
      />
      <p className="mt-4 text-sm text-gray-600">{siteUrl}</p>
    </div>
  );
}
```

### Styling
- Center the QR code on screen
- Use high contrast colors (black on white)
- Large size for projection visibility (512x512 minimum)
- Include URL text below QR code for reference

### Security Considerations
- Consider adding authentication if needed
- Keep route unlinked from main navigation
- Can be bookmarked for easy access

## Usage
1. Navigate to `/admin` route
2. QR code displays on screen
3. Project on church screens
4. Congregation scans QR code
5. Redirects to home page with verse
