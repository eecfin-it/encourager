import { useEffect } from 'react'
import confetti from 'canvas-confetti'

interface CelebrationProps {
  fire: boolean
}

export default function Celebration({ fire }: CelebrationProps) {
  useEffect(() => {
    if (!fire) return

    confetti({
      particleCount: 120,
      spread: 80,
      origin: { y: 0.7 },
      colors: ['#1a374f', '#6f9078', '#d06450', '#fdfbca', '#77c4f0'],
    })
  }, [fire])

  return null
}
