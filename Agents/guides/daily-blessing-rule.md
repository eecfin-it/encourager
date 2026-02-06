# Daily Blessing Rule: "One Blessing Per Day"

## Overview
The app must prevent users from generating multiple verses within the same calendar day to encourage reflection.

## Implementation Details

### Storage
- **Storage Key:** `last_blessing_data` in `localStorage`
- **Data Structure:** Object containing:
  - `timestamp`: ISO string of when they clicked "Amen"
  - `verse`: The text/reference they received (so they can still see it if they return)

### Workflow

#### Initialization
On component mount (`useEffect`), check if `last_blessing_data` exists in `localStorage`.

#### Date Comparison
- Compare the `timestamp` date with the current date
- If the date (Year-Month-Day) is the same as "Today", immediately state-shift to the **"Reflection Screen"**

#### The Reflection Screen
- Instead of a random verse, display: *"You have already received your Word for today. May it stay in your heart."*
- Re-display the verse they saved in `localStorage` so they can read it again
- Hide the "Amen" button
- Show a countdown timer: *"Next blessing available at midnight."*

#### The "Amen" Trigger
- Only save the timestamp to `localStorage` when the user clicks the "Amen" button
- This ensures that if they scan but don't finish reading, they aren't locked out

## Visual Feedback

### Reflection State
- Use a "Calm/Night" color palette (e.g., deep purples or soft greys)
- Distinguish the "Reflection" state from the "Active" state visually
- Softer, more contemplative design

### Active State
- Use the standard branding colors (see context/branding.md)
- Bright, encouraging design
- Clear call-to-action with "Amen" button

## Implementation Example

```typescript
// Check on mount
useEffect(() => {
  const lastBlessing = localStorage.getItem('last_blessing_data');
  if (lastBlessing) {
    const data = JSON.parse(lastBlessing);
    const lastDate = new Date(data.timestamp).toDateString();
    const today = new Date().toDateString();
    
    if (lastDate === today) {
      setShowReflection(true);
      setSavedVerse(data.verse);
    }
  }
}, []);

// Save on "Amen" click
const handleAmen = () => {
  const blessingData = {
    timestamp: new Date().toISOString(),
    verse: { text: verse.text, reference: verse.reference }
  };
  localStorage.setItem('last_blessing_data', JSON.stringify(blessingData));
  setAccepted(true);
};
```

## User Experience

### First Visit
1. User scans QR code
2. Verse loads and displays
3. User clicks "Amen"
4. Success screen shows
5. Blessing saved to localStorage

### Return Visit (Same Day)
1. User scans QR code or visits directly
2. Reflection screen immediately shows
3. Previous verse is displayed
4. Countdown timer shows time until midnight
5. No "Amen" button available

### Next Day
1. User scans QR code
2. New verse loads (date check passes)
3. Normal flow resumes
