import { motion } from 'framer-motion'
import { Sparkles } from 'lucide-react'

interface VerseDisplayProps {
  text: string
  reference: string
}

export default function VerseDisplay({ text, reference }: VerseDisplayProps) {
  return (
    <motion.div
      key={reference}
      initial={{ opacity: 0, y: 16 }}
      animate={{ opacity: 1, y: 0 }}
      transition={{ duration: 0.6, ease: 'easeOut' }}
      className="bg-white/80 backdrop-blur-sm rounded-[28px] shadow-lg border border-eccfin-green/20 px-8 py-10 max-w-sm w-full text-center"
    >
      <Sparkles className="w-6 h-6 text-eccfin-terracotta mx-auto mb-6" />

      <p className="text-xl leading-relaxed text-eccfin-navy font-serif italic">
        &ldquo;{text}&rdquo;
      </p>

      <p className="mt-5 text-sm font-semibold text-eccfin-slate tracking-wide uppercase">
        &mdash; {reference}
      </p>
    </motion.div>
  )
}
