#!/bin/bash
set -euo pipefail

REPO_ROOT="$(cd "$(dirname "$0")/.." && pwd)"
ENVIRONMENT=${1:-prod}
STACK_NAME=${STACK_NAME:-encourager-app}
AWS_REGION=${AWS_REGION:-eu-north-1}

echo "=========================================="
echo "Deploying Frontend to S3"
echo "=========================================="
echo "Environment: $ENVIRONMENT"
echo "Stack Name: $STACK_NAME"
echo "AWS Region: $AWS_REGION"
echo "=========================================="

echo ""
echo "Step 1: Getting S3 bucket name from CloudFormation stack..."
BUCKET_NAME=$(aws cloudformation describe-stacks \
  --stack-name ${STACK_NAME} \
  --query 'Stacks[0].Outputs[?OutputKey==`FrontendBucketName`].OutputValue' \
  --output text \
  --region ${AWS_REGION})

if [ -z "$BUCKET_NAME" ] || [ "$BUCKET_NAME" == "None" ]; then
    echo "Error: Could not find S3 bucket name from stack ${STACK_NAME}"
    echo "Make sure the backend has been deployed first."
    exit 1
fi

echo "S3 Bucket: $BUCKET_NAME"

echo ""
echo "Step 2: Building frontend..."
cd "${REPO_ROOT}/frontend"
npm ci
npm run build

if [ ! -d "dist" ]; then
    echo "Error: dist directory not found. Build may have failed."
    exit 1
fi

echo ""
echo "Step 3: Uploading static assets to S3 (with long cache, deleting removed files)..."
aws s3 sync dist/ s3://${BUCKET_NAME}/ \
  --delete \
  --cache-control "public, max-age=31536000, immutable" \
  --exclude "*.html" \
  --exclude "*.json" \
  --region ${AWS_REGION}

echo ""
echo "Step 4: Uploading HTML files (no cache)..."
aws s3 cp dist/ s3://${BUCKET_NAME}/ \
  --recursive \
  --cache-control "public, max-age=0, must-revalidate" \
  --exclude "*" \
  --include "*.html" \
  --region ${AWS_REGION}

echo ""
echo "Step 5: Uploading JSON files (short cache)..."
aws s3 cp dist/ s3://${BUCKET_NAME}/ \
  --recursive \
  --cache-control "public, max-age=3600" \
  --exclude "*" \
  --include "*.json" \
  --region ${AWS_REGION}

echo ""
echo "Step 6: Getting CloudFront distribution ID..."
DISTRIBUTION_ID=$(aws cloudformation describe-stacks \
  --stack-name ${STACK_NAME} \
  --query 'Stacks[0].Outputs[?OutputKey==`CloudFrontDistributionId`].OutputValue' \
  --output text \
  --region ${AWS_REGION})

if [ ! -z "$DISTRIBUTION_ID" ] && [ "$DISTRIBUTION_ID" != "None" ]; then
    echo "CloudFront Distribution ID: $DISTRIBUTION_ID"
    echo ""
    echo "Step 7: Invalidating CloudFront cache..."
    INVALIDATION_ID=$(aws cloudfront create-invalidation \
      --distribution-id ${DISTRIBUTION_ID} \
      --paths "/*" \
      --query 'Invalidation.Id' \
      --output text \
      --region ${AWS_REGION})
    echo "Invalidation created: $INVALIDATION_ID"
    echo "Note: Cache invalidation may take a few minutes to complete."
else
    echo "Warning: Could not find CloudFront distribution ID. Skipping cache invalidation."
fi

echo ""
echo "=========================================="
echo "Frontend deployment complete!"
echo "=========================================="
CLOUDFRONT_URL=$(aws cloudformation describe-stacks \
  --stack-name ${STACK_NAME} \
  --query 'Stacks[0].Outputs[?OutputKey==`CloudFrontUrl`].OutputValue' \
  --output text \
  --region ${AWS_REGION})
echo "CloudFront URL: https://${CLOUDFRONT_URL}"
echo "=========================================="
