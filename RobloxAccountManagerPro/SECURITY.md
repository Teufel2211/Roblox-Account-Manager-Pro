# Security Policy

## Reporting Security Vulnerabilities

**Do not** open public GitHub issues for security vulnerabilities. Instead, please email security@example.com with:

1. Description of the vulnerability
2. Steps to reproduce
3. Potential impact
4. Suggested fix (if any)

We will acknowledge receipt within 48 hours and provide a timeline for a fix.

## Security Features

### Data Encryption
- **Passwords**: AES-256 encryption with PBKDF2 key derivation
- **Local Storage**: Windows DPAPI protection
- **Transport**: HTTPS for all cloud communications

### Authentication
- Master password with 10,000 PBKDF2 iterations
- Windows Hello biometric support
- Session-based security with auto-lock

### Best Practices
- No passwords stored in plaintext
- Secure memory wiping for sensitive data
- Minimal permissions model
- Activity logging for audit trails

## Supported Versions

| Version | Status | Security Updates |
|---------|--------|------------------|
| 2.x     | Current | Yes |
| 1.x     | Legacy | No |

## Security Updates

Security updates are released as soon as they are available. Users are encouraged to update immediately when a security patch is released.
