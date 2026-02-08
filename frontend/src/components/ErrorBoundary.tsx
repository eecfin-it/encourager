import { Component } from 'react'
import type { ErrorInfo, ReactNode } from 'react'

interface Props {
  children: ReactNode
}

interface State {
  hasError: boolean
}

export class ErrorBoundary extends Component<Props, State> {
  constructor(props: Props) {
    super(props)
    this.state = { hasError: false }
  }

  static getDerivedStateFromError(): State {
    return { hasError: true }
  }

  componentDidCatch(error: Error, info: ErrorInfo) {
    console.error('ErrorBoundary caught:', error, info)
  }

  render() {
    if (this.state.hasError) {
      return (
        <div className="min-h-dvh flex flex-col items-center justify-center px-6 bg-eccfin-cream">
          <h1 className="text-xl font-bold text-eccfin-navy mb-4">Something went wrong</h1>
          <p className="text-eccfin-slate mb-6 text-center">
            Please refresh the page to try again.
          </p>
          <button
            onClick={() => window.location.reload()}
            className="px-6 py-3 bg-eccfin-navy text-white rounded-lg"
          >
            Refresh
          </button>
        </div>
      )
    }

    return this.props.children
  }
}
