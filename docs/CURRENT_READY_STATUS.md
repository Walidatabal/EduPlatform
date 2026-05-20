# EduPlatform Current Ready Status

## Ready now

- Local Docker stack
- API with Swagger
- MVC Web app
- SQL Server Docker database
- Redis infrastructure
- Seq logging infrastructure
- JWT login
- Refresh token persistence
- Role-based authorization
- Notifications foundation
- Live sessions / attendance foundation
- Production-safe config templates
- `.env.example`
- Production migration docs

## Not included in GitHub package

The following are intentionally excluded:

- `.env`
- `.git`
- `.vs`
- `bin`
- `obj`
- logs

## Important note

The app is portfolio/demo ready. Real production readiness requires external services to be configured:

- Azure SQL
- Azure App Service
- Email SMTP/SendGrid
- Blob Storage
- Payment provider
