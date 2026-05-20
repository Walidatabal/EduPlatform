# EduPlatform External Services Implementation Guide

These services are required to reach a 9.5+/10 enterprise GitHub score.

## 1. Email Service

Recommended first provider: MailKit + SMTP.

Use cases:
- forgot password
- email verification
- welcome email
- session reminder

Required config:

```text
EmailSettings__Host
EmailSettings__Port
EmailSettings__Username
EmailSettings__Password
EmailSettings__From
```

Do not store email passwords in GitHub.

## 2. Azure Blob Storage

Use for:
- course thumbnails
- certificate PDFs
- lesson attachments
- live session recordings

Required config:

```text
BlobStorage__ConnectionString
BlobStorage__ContainerName
```

## 3. Payment Gateway

Recommended:
- MyFatoorah for Kuwait/GCC
- Stripe for international demos

Payment flow:

```text
Create Order → Create Payment Session → Receive Webhook → Verify Payment → Mark Paid → Enroll Student → Send Email → Create Notification
```

## 4. SignalR

Use for:
- real-time notification badge
- live session alerts
- admin announcements

## 5. Hangfire

Use for:
- email queue
- reminders
- certificate generation
- cleanup jobs

## 6. QuestPDF

Use for:
- certificate PDF generation
- QR verification codes
- branded certificate template
```

