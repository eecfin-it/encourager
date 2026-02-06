# Encourager

A scripture verse encouragement app that serves random Bible verses in English, Amharic, and Finnish. Built with a .NET 10 backend API and a React + TypeScript frontend.

## Architecture

```
                  ┌──────────────┐
                  │  CloudFront  │
                  │    (CDN)     │
                  └──────┬───────┘
                         │
              ┌──────────┴──────────┐
              │                     │
     /api/* requests          Static assets
              │                     │
   ┌──────────▼──────────┐  ┌──────▼──────┐
   │   API Gateway       │  │  S3 Bucket  │
   │   (REST API)        │  │  (Private)  │
   └──────────┬──────────┘  └─────────────┘
              │
   ┌──────────▼──────────┐
   │   AWS Lambda        │
   │   (.NET 10)         │
   └─────────────────────┘
```

- **Frontend**: React 19 + TypeScript + Tailwind CSS 4, served from S3 via CloudFront
- **Backend**: .NET 10 minimal API running on AWS Lambda (container image)
- **Infrastructure**: AWS SAM (CloudFormation), deployed via shell scripts or GitHub Actions CI/CD

## Prerequisites

| Tool              | Version | Install (macOS)              |
| ----------------- | ------- | ---------------------------- |
| .NET SDK          | 10.0+   | `brew install dotnet`        |
| Node.js           | 22+     | `brew install node`          |
| AWS CLI           | 2.x     | `brew install awscli`        |
| AWS SAM CLI       | 1.x     | `brew install aws-sam-cli`   |
| Docker Desktop    | latest  | https://docker.com/products/docker-desktop |

## Project Structure

```
.
├── backend/
│   ├── AppConfiguration.cs      # Shared service & endpoint registration
│   ├── Program.cs               # Local development entry point
│   ├── LambdaEntryPoint.cs      # AWS Lambda entry point
│   ├── Dockerfile.lambda        # Lambda container image
│   ├── Services/
│   │   └── VerseService.cs      # Verse retrieval logic
│   ├── Data/                    # Verse data (English, Amharic, Finnish)
│   └── Models/
│       └── Verse.cs
├── frontend/
│   ├── src/
│   │   ├── components/          # React components
│   │   ├── contexts/            # Language context
│   │   ├── pages/               # Page components
│   │   └── i18n/                # Translations
│   ├── package.json
│   └── vite.config.ts
├── infrastructure/
│   ├── template.yaml            # SAM/CloudFormation template
│   └── samconfig.toml.example   # SAM config example
├── scripts/
│   ├── deploy-backend.sh        # Backend deployment script
│   └── deploy-frontend.sh       # Frontend deployment script
└── .github/workflows/
    └── deploy.yml               # CI/CD pipeline
```

## Local Development

### 1. Start the Backend

```bash
cd backend
dotnet restore
dotnet run
```

The API will be available at `http://localhost:5226`.

### 2. Start the Frontend

```bash
cd frontend
npm install
npm run dev
```

The frontend will be available at `http://localhost:5173`. API calls to `/api/*` are proxied to the backend automatically via Vite.

### 3. Test the API

```bash
# Random verse (English)
curl http://localhost:5226/api/verse/random

# Random verse (Amharic)
curl http://localhost:5226/api/verse/random?lang=am

# Specific verse by index
curl http://localhost:5226/api/verse/random?lang=en&index=5

# Health check
curl http://localhost:5226/api/health
```

## API Endpoints

| Method | Path               | Description                      | Query Params              |
| ------ | ------------------ | -------------------------------- | ------------------------- |
| GET    | `/api/verse/random`| Get a random (or specific) verse | `lang` (en/am/fi), `index` |
| GET    | `/api/health`      | Health check endpoint            | —                         |

## Publishing to AWS

### Initial AWS Setup

1. **Configure AWS CLI** with your credentials:

   ```bash
   aws configure
   ```

2. **Set up SAM config** by copying the example:

   ```bash
   cp infrastructure/samconfig.toml.example infrastructure/samconfig.toml
   ```

   Edit `infrastructure/samconfig.toml` if you need a different region or stack name.

3. **Ensure Docker Desktop is running** (required for building Lambda container images).

### Option 1: Manual Deployment

#### Deploy the Backend

```bash
./scripts/deploy-backend.sh
```

This will:
1. Build a Docker image for the Lambda function
2. Tag it with the current git commit SHA for traceability
3. Push the image to Amazon ECR
4. Deploy the SAM stack (Lambda, API Gateway, S3 bucket, CloudFront distribution)

#### Deploy the Frontend

```bash
./scripts/deploy-frontend.sh prod
```

This will:
1. Build the React frontend (`npm ci && npm run build`)
2. Upload static assets to S3 with appropriate cache headers
3. Invalidate the CloudFront cache

#### Customizing the Deployment

You can override defaults with environment variables:

```bash
export AWS_REGION=eu-west-1
export STACK_NAME=my-encourager-app
./scripts/deploy-backend.sh
./scripts/deploy-frontend.sh prod
```

You can also pass SAM parameter overrides for CORS and image tag:

```bash
sam deploy \
  --parameter-overrides \
    AllowedOrigin=https://d1234abcdef.cloudfront.net \
    ImageTag=$(git rev-parse --short HEAD)
```

### Option 2: GitHub Actions CI/CD

The repository includes a GitHub Actions workflow that deploys on every push to `main`.

#### Setup

1. **Create an IAM OIDC identity provider** in your AWS account for GitHub Actions:

   ```bash
   aws iam create-open-id-connect-provider \
     --url https://token.actions.githubusercontent.com \
     --client-id-list sts.amazonaws.com \
     --thumbprint-list 6938fd4d98bab03faadb97b34396831e3780aea1
   ```

2. **Create an IAM role** that GitHub Actions can assume. The trust policy should look like:

   ```json
   {
     "Version": "2012-10-17",
     "Statement": [
       {
         "Effect": "Allow",
         "Principal": {
           "Federated": "arn:aws:iam::YOUR_ACCOUNT_ID:oidc-provider/token.actions.githubusercontent.com"
         },
         "Action": "sts:AssumeRoleWithWebIdentity",
         "Condition": {
           "StringEquals": {
             "token.actions.githubusercontent.com:aud": "sts.amazonaws.com"
           },
           "StringLike": {
             "token.actions.githubusercontent.com:sub": "repo:YOUR_ORG/Encourager:*"
           }
         }
       }
     ]
   }
   ```

   Attach policies granting access to CloudFormation, S3, ECR, Lambda, API Gateway, CloudFront, and IAM (for SAM deployments).

3. **Add the GitHub secret**:
   - Go to **Settings > Secrets and variables > Actions** in your repository
   - Add `AWS_ROLE_ARN` with the ARN of the IAM role you created

4. **Push to main** to trigger deployment:

   ```bash
   git push origin main
   ```

The workflow deploys the backend first, then the frontend.

### Verifying the Deployment

After deployment, retrieve your URLs:

```bash
# CloudFront URL (your app)
aws cloudformation describe-stacks \
  --stack-name encourager-app \
  --query 'Stacks[0].Outputs[?OutputKey==`CloudFrontUrl`].OutputValue' \
  --output text

# API Gateway URL (direct API access)
aws cloudformation describe-stacks \
  --stack-name encourager-app \
  --query 'Stacks[0].Outputs[?OutputKey==`ApiUrl`].OutputValue' \
  --output text
```

Test the deployment:

```bash
# Test health endpoint via CloudFront
curl https://YOUR_CLOUDFRONT_URL/api/health

# Test verse endpoint via CloudFront
curl https://YOUR_CLOUDFRONT_URL/api/verse/random

# Open the app in your browser
open https://YOUR_CLOUDFRONT_URL
```

### Restricting CORS for Production

By default, CORS allows all origins (`*`). After your first deployment, restrict it to your CloudFront domain:

```bash
sam deploy \
  --stack-name encourager-app \
  --parameter-overrides AllowedOrigin=https://YOUR_CLOUDFRONT_URL \
  --capabilities CAPABILITY_IAM \
  --no-confirm-changeset \
  --no-fail-on-empty-changeset
```

### Updating an Existing Deployment

After making code changes, redeploy whichever part changed:

```bash
# Backend changes
./scripts/deploy-backend.sh

# Frontend changes
./scripts/deploy-frontend.sh prod

# Both
./scripts/deploy-backend.sh && ./scripts/deploy-frontend.sh prod
```

Each backend deployment creates a new image tagged with the git SHA, making it easy to identify and roll back to previous versions.

## Monitoring

```bash
# View Lambda logs (live tail)
aws logs tail /aws/lambda/encourager-app-VerseApiFunction --follow

# Check stack status
aws cloudformation describe-stacks --stack-name encourager-app

# Check CloudFront distribution
aws cloudfront list-distributions \
  --query "DistributionList.Items[?Comment=='Encourager App Distribution']"
```

## Cleanup

To tear down all AWS resources:

```bash
# Delete the CloudFormation stack (removes Lambda, API Gateway, S3, CloudFront)
aws cloudformation delete-stack --stack-name encourager-app

# Wait for deletion to complete
aws cloudformation wait stack-delete-complete --stack-name encourager-app

# Delete the ECR repository
aws ecr delete-repository --repository-name encourager-api --force --region us-east-1
```

## Cost Estimate

| Traffic Level     | Requests/month | Estimated Cost |
| ----------------- | -------------- | -------------- |
| Low               | 10K            | ~$0.50/month   |
| Moderate          | 100K           | ~$5/month      |
| High              | 1M+            | ~$8–15/month   |

Most usage falls within the AWS Free Tier (1M Lambda invocations, 1M API Gateway requests, 5 GB S3, 1 TB CloudFront transfer per month).
