# Production Security Checklist

## 1. Secrets

Never commit real secrets.

Move these to environment variables or a secret manager:

```text
SQL password
JWT signing key
Admin password
SMTP credentials
Payment gateway keys
```

Recommended production options:

```text
Azure Key Vault
AWS Secrets Manager
Docker secrets
Kubernetes secrets
Environment variables
```

---

## 2. JWT Security

Current project uses JWT Bearer authentication for API.

Production checklist:

```text
Use a long random signing key
Short access token lifetime
Refresh token rotation
Store refresh token securely
Revoke refresh tokens on password change
Validate issuer and audience
Use HTTPS only
```

---

## 3. Cookie Security

MVC portal uses Identity cookies.

Production checklist:

```text
HttpOnly = true
SecurePolicy = Always
SameSite = Lax or Strict
Sliding expiration enabled
Lockout enabled
Password reset flow enabled
```

---

## 4. Role and Permission Security

Current project supports role-based access.

Next step:

```text
Permission-based authorization
Policy-based authorization
Audit logs for admin actions
```

Examples:

```text
CanManageUsers
CanApproveCourses
CanResetPasswords
CanViewReports
```

---

## 5. CSRF Protection

MVC forms should use:

```csharp
[ValidateAntiForgeryToken]
```

All state-changing MVC actions should be POST + anti-forgery protected.

---

## 6. CORS

Do not use:

```text
AllowAnyOrigin
```

Production CORS should allow only known frontend domains.

---

## 7. Admin Account Recovery

AdminSeeder resets the admin account for development/demo.

For production:

```text
Disable automatic password reset
Use secure emergency recovery procedure
Require strong password and MFA
```

---

## 8. Audit Logging

Important admin actions should be logged:

```text
User created
Role changed
Account unlocked
Password reset
Course approved/rejected
Payment status changed
```

---

# Final Recommendation

For portfolio/demo, current security is strong.

For production, add:

```text
Refresh tokens
Email confirmation
Forgot/reset password email
MFA
Audit logs
Permission-based authorization
Secret manager
```
