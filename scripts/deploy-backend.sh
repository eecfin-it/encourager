#!/bin/bash
set -euo pipefail

AWS_REGION=${AWS_REGION:-eu-north-1}
AWS_ACCOUNT_ID=$(aws sts get-caller-identity --query Account --output text)
ECR_REPO="encourager-api"
IMAGE_TAG=$(git rev-parse --short HEAD)
STACK_NAME=${STACK_NAME:-encourager-app}

if [ -z "$AWS_ACCOUNT_ID" ]; then
    echo "Error: Unable to get AWS Account ID. Make sure AWS CLI is configured."
    exit 1
fi

echo "=========================================="
echo "Deploying Backend to AWS Lambda"
echo "=========================================="
echo "AWS Account ID: $AWS_ACCOUNT_ID"
echo "AWS Region: $AWS_REGION"
echo "ECR Repository: $ECR_REPO"
echo "Image Tag: $IMAGE_TAG"
echo "Stack Name: $STACK_NAME"
echo "=========================================="

echo ""
echo "Step 1: Building Docker image..."
cd "$(dirname "$0")/../backend"
docker build -f Dockerfile.lambda -t ${ECR_REPO}:${IMAGE_TAG} .

echo ""
echo "Step 2: Logging into ECR..."
aws ecr get-login-password --region ${AWS_REGION} | docker login --username AWS --password-stdin ${AWS_ACCOUNT_ID}.dkr.ecr.${AWS_REGION}.amazonaws.com

echo ""
echo "Step 3: Creating ECR repository if it doesn't exist..."
aws ecr describe-repositories --repository-names ${ECR_REPO} --region ${AWS_REGION} 2>/dev/null || \
  aws ecr create-repository --repository-name ${ECR_REPO} --region ${AWS_REGION} --image-scanning-configuration scanOnPush=true

ECR_URI="${AWS_ACCOUNT_ID}.dkr.ecr.${AWS_REGION}.amazonaws.com/${ECR_REPO}:${IMAGE_TAG}"
echo ""
echo "Step 4: Tagging and pushing image to ${ECR_URI}..."
docker tag ${ECR_REPO}:${IMAGE_TAG} ${ECR_URI}
docker push ${ECR_URI}

echo ""
echo "Step 5: Deploying with SAM..."
cd "$(dirname "$0")/../infrastructure"
sam build
sam deploy \
  --stack-name ${STACK_NAME} \
  --image-repository ${ECR_URI%:*} \
  --region ${AWS_REGION} \
  --capabilities CAPABILITY_IAM \
  --no-confirm-changeset \
  --no-fail-on-empty-changeset \
  --parameter-overrides ImageTag=${IMAGE_TAG}

echo ""
echo "=========================================="
echo "Backend deployment complete!"
echo "=========================================="
echo "API URL: $(aws cloudformation describe-stacks --stack-name ${STACK_NAME} --query 'Stacks[0].Outputs[?OutputKey==`ApiUrl`].OutputValue' --output text --region ${AWS_REGION})"
echo "CloudFront URL: $(aws cloudformation describe-stacks --stack-name ${STACK_NAME} --query 'Stacks[0].Outputs[?OutputKey==`CloudFrontUrl`].OutputValue' --output text --region ${AWS_REGION})"
echo "=========================================="
