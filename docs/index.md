# Encourager - Scripture Verse Encouragement App

Welcome to the Encourager documentation! This PWA serves random Bible verses in English, Amharic, and Finnish, encouraging daily reflection through a "one blessing per day" rule.

## Quick Links

- [Getting Started](developer/getting-started.md) - Set up your development environment
- [Architecture Overview](architecture/system-architecture.md) - Understand the system design
- [API Reference](developer/api-reference.md) - Backend API endpoints
- [User Guide](user/user-guide.md) - How to use the app
- [Deployment Guide](developer/deployment.md) - Deploy to AWS

## Project Overview

Encourager is a mobile-first Progressive Web App (PWA) that delivers daily scripture verses to church members via QR code scanning. The app enforces a "one blessing per day" rule — users receive one verse and must click "Amen" to lock it in; subsequent visits that day show a reflection view with countdown to midnight.

### Key Features

- 🌍 **Multi-language Support**: English, Amharic (አማርኛ), and Finnish
- 📱 **Mobile-First PWA**: Installable on devices, works offline
- ⏰ **Daily Limit**: One blessing per day encourages reflection
- 🎨 **Beautiful UI**: Modern, spiritual design with smooth animations
- 🔄 **QR Code Integration**: Easy sharing via QR codes

### Tech Stack

- **Frontend**: React 19 + TypeScript + Tailwind CSS 4 + Vite
- **Backend**: .NET 10 Minimal APIs (C# 14)
- **Infrastructure**: AWS (Lambda, API Gateway, S3, CloudFront)
- **Deployment**: AWS SAM (CloudFormation) + GitHub Actions CI/CD

## Documentation Structure

### Architecture
- [System Architecture](architecture/system-architecture.md)
- [Component Relationships](architecture/component-relationships.md)
- [Data Flow](architecture/data-flow.md)
- [Deployment Architecture](architecture/deployment-architecture.md)

### User Documentation
- [User Guide](user/user-guide.md)
- [Features](user/features.md)
- [Troubleshooting](user/troubleshooting.md)

### Developer Documentation
- [Getting Started](developer/getting-started.md)
- [API Reference](developer/api-reference.md)
- [Development Workflows](developer/development-workflows.md)
- [Testing Guidelines](developer/testing-guidelines.md)
- [Deployment](developer/deployment.md)
- [Contributing](developer/contributing.md)

## Support

For issues, questions, or contributions, please refer to the [Contributing Guide](developer/contributing.md) or open an issue in the repository.
