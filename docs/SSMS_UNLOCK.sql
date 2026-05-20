-- ============================================================
-- EduPlatform — Admin Account Unlock & Password Reset
-- Run this in SSMS if you cannot log in as admin@eduplatform.com
-- ============================================================

-- Step 1: Verify current state
SELECT
    Id,
    Email,
    PasswordHash,
    LockoutEnabled,
    LockoutEnd,
    AccessFailedCount,
    EmailConfirmed
FROM AspNetUsers
WHERE Email = 'admin@eduplatform.com';

-- Step 2: Clear lockout
UPDATE AspNetUsers
SET
    LockoutEnd         = NULL,
    AccessFailedCount  = 0,
    LockoutEnabled     = 0,
    EmailConfirmed     = 1
WHERE Email = 'admin@eduplatform.com';

-- Step 3: Verify the fix
SELECT Email, LockoutEnabled, LockoutEnd, AccessFailedCount
FROM AspNetUsers
WHERE Email = 'admin@eduplatform.com';

-- ============================================================
-- IMPORTANT: Do NOT manually write a PasswordHash into SQL.
-- ASP.NET Identity uses a hashed+salted format that cannot be
-- hand-crafted. After running the UPDATE above:
--
--   Option A (recommended): Restart the API/Docker. The AdminSeeder
--   runs on every startup and automatically resets the password to
--   whatever is in Seeding:AdminPassword in appsettings.json.
--   Default: Admin@123456
--
--   Option B: Log in to the MVC portal as admin and use
--   /Account/ChangePassword to set a new password interactively.
--
--   Option C: Use /Account/ResetPassword/{id} (Admin only) to
--   reset another user's password from within the portal.
-- ============================================================
