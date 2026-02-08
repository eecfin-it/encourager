# CORS Configuration Guide

## Overview

The Encourager API implements environment-aware CORS (Cross-Origin Resource Sharing) configuration to ensure security in production while maintaining developer convenience in local development.

## Configuration Behavior

### Development Environment

When `ASPNETCORE_ENVIRONMENT=Development`:
- **No `ALLOWED_ORIGIN` set**: Allows all origins (`*`) for easy local development
- **`ALLOWED_ORIGIN` set**: Uses the specified origin(s)

### Production Environment

When `ASPNETCORE_ENVIRONMENT=Production` (or any non-Development value):
- **No `ALLOWED_ORIGIN` set**: **Application will fail to start** with a clear error message
- **`ALLOWED_ORIGIN` set**: Uses the specified origin(s)
- **`ALLOWED_ORIGIN="*"`**: Allows all origins but logs a warning (not recommended)

## Setting CORS Origins

### Single Origin

```bash
export ALLOWED_ORIGIN="https://d1234567890.cloudfront.net"
```

### Multiple Origins

Separate multiple origins with commas:

```bash
export ALLOWED_ORIGIN="https://encourager.example.com,https://www.encourager.example.com"
```

## AWS Deployment

### Automatic CloudFront Configuration

The SAM template automatically configures CORS to use the CloudFront distribution URL:

```yaml
# infrastructure/template.yaml
ALLOWED_ORIGIN: !If 
  - HasCustomOrigin
  - !Ref AllowedOrigin
  - !Sub "https://${CloudFrontDistribution.DomainName}"
```

**Default behavior**: If you don't specify `AllowedOrigin` parameter during deployment, it will automatically use the CloudFront URL.

### Custom Domain Deployment

If you're using a custom domain, override the `AllowedOrigin` parameter:

```bash
sam deploy \
  --stack-name encourager-app \
  --parameter-overrides \
    AllowedOrigin="https://encourager.example.com" \
    Environment=prod
```

### Multiple Environments

For staging and production with different domains:

```bash
# Staging
sam deploy \
  --stack-name encourager-staging \
  --parameter-overrides \
    AllowedOrigin="https://staging.encourager.example.com" \
    Environment=staging

# Production
sam deploy \
  --stack-name encourager-prod \
  --parameter-overrides \
    AllowedOrigin="https://encourager.example.com,https://www.encourager.example.com" \
    Environment=prod
```

## Local Development

### Using Docker Compose

The `docker-compose.yml` automatically sets `ASPNETCORE_ENVIRONMENT=Development`, allowing all origins.

### Using Kestrel (dotnet run)

By default, `appsettings.Development.json` is used, setting the environment to Development:

```bash
cd backend
dotnet run
# CORS will allow all origins in Development mode
```

### Testing with Specific Origin

To test CORS with a specific origin locally:

```bash
export ASPNETCORE_ENVIRONMENT=Production
export ALLOWED_ORIGIN="http://localhost:5173"
cd backend
dotnet run
```

## Troubleshooting

### Error: "ALLOWED_ORIGIN environment variable must be set"

**Cause**: Running in Production mode without setting `ALLOWED_ORIGIN`.

**Solution**: Set the environment variable:
```bash
export ALLOWED_ORIGIN="https://your-domain.com"
```

### CORS Errors in Browser Console

**Symptom**: `Access to fetch at 'https://api.example.com/api/verse/random' from origin 'https://app.example.com' has been blocked by CORS policy`

**Solutions**:
1. Verify `ALLOWED_ORIGIN` includes your frontend domain
2. Check that the protocol (http/https) matches exactly
3. Ensure no trailing slashes in the origin URL
4. For multiple origins, verify comma separation with no spaces

### Warning: "CORS is configured with wildcard (*)"

**Cause**: `ALLOWED_ORIGIN` is explicitly set to `"*"` in production.

**Impact**: All origins can access your API (security risk).

**Solution**: Set a specific origin:
```bash
export ALLOWED_ORIGIN="https://your-cloudfront-url.cloudfront.net"
```

## Security Best Practices

1. ✅ **Never use `*` in production** - Always specify exact origins
2. ✅ **Use HTTPS** - CloudFront enforces HTTPS by default
3. ✅ **Limit origins** - Only allow your frontend domain(s)
4. ✅ **Review regularly** - Audit CORS configuration during security reviews
5. ✅ **Test thoroughly** - Verify CORS works before deploying to production

## Implementation Details

The CORS configuration is implemented in `backend/AppConfiguration.cs`:

```csharp
var allowedOrigin = Environment.GetEnvironmentVariable("ALLOWED_ORIGIN");
var aspNetCoreEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

if (string.IsNullOrEmpty(allowedOrigin))
{
    if (aspNetCoreEnvironment.Equals("Development", StringComparison.OrdinalIgnoreCase))
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    }
    else
    {
        throw new InvalidOperationException("ALLOWED_ORIGIN must be set in production");
    }
}
```

## Related Documentation

- [AWS SAM Template](../../infrastructure/template.yaml)
- [Backend Configuration](../../backend/AppConfiguration.cs)
- [Deployment Scripts](../../scripts/deploy-backend.sh)

