# Testing Expansion Guide

## Goal

The current tests are useful for DTOs, enums, result objects, and ViewModels. The next enterprise step is to test business workflows and API behavior.

---

# Recommended Test Layers

## 1. Unit Tests

Use for pure logic with no real database.

Examples:

```text
Course validation
Coupon calculations
Cart total calculations
Role rules
ViewModel defaults
Result wrappers
```

---

## 2. Service Tests

Use mocked repositories / UnitOfWork.

Important services to test:

```text
CourseService
CartService
WishlistService
OrderService
EnrollmentService
NotificationService
CertificateService
```

### Example Test Idea

```text
AddToCartAsync should fail if course does not exist.
AddToCartAsync should prevent duplicate cart items.
Checkout should create order and clear cart.
Enrollment should prevent duplicate enrollment.
```

---

## 3. Repository Tests

Use a real test database or SQLite only if behavior is simple.

Recommended enterprise option:

```text
Testcontainers + SQL Server
```

Why?

Because EF Core behavior can differ between in-memory providers and SQL Server.

---

## 4. API Integration Tests

Use:

```text
Microsoft.AspNetCore.Mvc.Testing
WebApplicationFactory
Testcontainers SQL Server
```

Test cases:

```text
POST /api/auth/login returns JWT
POST /api/courses without token returns 401
POST /api/courses with wrong role returns 403
POST /api/courses with admin token succeeds
GET /health returns 200
```

---

## 5. MVC Integration Tests

Test:

```text
Login page loads
Dashboard redirects unauthenticated users
Admin users can open /Account/Users
Students cannot open admin pages
```

---

# Priority Test Plan

## Phase 1

```text
Auth tests
CourseService tests
CartService tests
OrderService tests
```

## Phase 2

```text
Repository tests
API integration tests
MVC role access tests
```

## Phase 3

```text
Docker integration tests
End-to-end checkout flow
Performance smoke tests
```

---

# Interview Explanation

The project already has a unit testing foundation. The next professional improvement is adding integration tests using WebApplicationFactory and Testcontainers so the API is validated against a real SQL Server environment.
