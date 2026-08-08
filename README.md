# Gas Station Billing System

Full project: backend in C# (ASP.NET Core + SQLite) and frontend in React (Vite).
The app UI can switch between English and Spanish from **Options**.

## Structure

```
GasStationBilling.Api/   ← Backend
frontend/                 ← React frontend
install-everything.bat    ← Double-click: installs everything needed (new computer)
start.bat                 ← Double-click: runs backend + frontend together
```

## First time on a new computer

1. Double-click **`install-everything.bat`**
   - Checks whether you have .NET 8 and Node.js installed; installs them
     automatically if missing (uses `winget`, built into Windows 10/11)
   - If it installed something new, it'll ask you to close and reopen the terminal
   - Installs the project's dependencies (NuGet + npm) and creates the database

If `install-everything.bat` seems stuck right after installing something new,
close it and double-click it again (so it picks up the newly installed tools).

## Everyday use (already installed)

Double-click **`start.bat`**. This opens 2 windows (backend and frontend) and
opens your browser automatically. To shut everything down, close those 2 windows.

## Manual setup (2 terminals)

**Terminal 1:**
```powershell
cd GasStationBilling.Api
dotnet run
```

**Terminal 2:**
```powershell
cd frontend
npm run dev
```

Open `http://localhost:5173`.

## Test accounts

| Username | Password | Permissions |
|---|---|---|
| `user1` | `123456` | Dispenses fuel (sales, purchases) |
| `admin1` | `admin123` | Everything above + create new users |

**Change these passwords** before using the system with real money — you can
create a new admin from the **Options** screen and then stop using the test ones.

## What's on each screen

- **Station**: gauge for each tank + today's sales summary
- **Sale**: register fuel dispensed to a customer
- **Purchase**: register a refill from the supplier
- **Prices**: sale/purchase price and **tank capacity** for each product
- **Reports**: purchase/sales summary by date range
- **Options**:
  - Day/night theme (any user)
  - **English/Spanish language switch** (any user)
  - Station name (shown in the header)
  - Create new users — **only visible if you signed in with an admin account**

## Notes on admin permissions

This system doesn't use session tokens (JWT) — it's intentionally simple for
a single point of sale. The "only admin can create users" check happens on
the backend, by confirming that the `EmployeeId` that logged in really has
`IsAdmin = true`. That's enough for a small business with trusted staff, but
if you ever expose this system to the internet (not just your local network),
let me know and we'll harden it with real authentication.

## Possible next steps

- Sales history with ticket printing
- Export reports to Excel/PDF
- Alerts when a tank drops below a certain level
