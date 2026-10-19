# 🚀 VelopackUpdateTest

A modern, high-performance .NET 9 Windows Forms application demonstrating full **Velopack** auto-update integration, complete update simulation sandbox, rich version changelog explorer, and automated GitHub Actions CI/CD workflows.

---

## 🌟 Highlights & Features

- **⚡ Native Velopack Integration**:
  - Initialized with `VelopackApp.Build().OnFirstRun(...).OnRestarted(...).Run()` in `Program.cs`.
  - Integrates with `Velopack.UpdateManager` for real GitHub Releases distribution.
  - Automatically identifies whether running from an installed package directory or from local `bin/Debug`.

- **🧪 Full Interactive Simulation Sandbox**:
  - Test update notifications, download progress, and restart behaviors without publishing real releases to GitHub.
  - **Scenario Presets**:
    - **v1.1.0 Feature Pack** (Optional Minor Update with Delta compression)
    - **v2.0.0 Major Overhaul (🚨 Mandatory Force Update)** (Locks dismissal, disables skip, and requires user to update)
    - **v1.0.1 Stability Patch** (Fast delta update)
  - **Network Failure Simulation**: Toggle 50% simulated connection drops to test retry behavior and user-facing error handling.

- **🎨 Modern Dark UI & Cool Update Screen (`UpdateDialogForm`)**:
  - Double-buffered GDI+ controls with zero flicker and high-DPI scaling.
  - Glowing header badges, version flow transitions (`v1.0.0 ──► v2.0.0`), and metadata tags.
  - **Rich Formatted Changelog**: Categorized sections for *✨ New Features*, *⚡ Improvements*, *🐛 Bug Fixes*, and *🔒 Security Enhancements*.
  - **Smooth Progress Bar**: Real-time download speeds (MB/s), downloaded megabytes, and phase status.
  - **Forced Update Policy Enforcement**: Prevents dialog dismissal or app usage until the update is installed. If closed, confirms app shutdown.

- **📋 Version & Changelog Explorer**:
  - View full release notes for the current installed version (`v1.0.0`), previous beta versions (`v0.9.0-beta`), and future release previews (`v1.1.0`, `v2.0.0`).

- **📟 Real-Time Activity Console**:
  - Live timestamped logging of all Velopack locator queries, network requests, download steps, and lifecycle events with one-click clipboard copying.

- **🤖 Automated GitHub Actions Workflow**:
  - Full CI/CD via `.github/workflows/build-and-release.yml`.
  - Automatically builds a self-contained `win-x64` binary, invokes `vpk pack` to generate setup installers and delta packages, and uploads everything directly to GitHub Releases.

---

## 🏗️ Project Architecture

```
WinFormsApp1/
├── Models/
│   └── ReleaseInfo.cs            # Version models, changelog history, & preset scenarios
├── Services/
│   └── VelopackUpdateService.cs  # Unified service handling both real Velopack API & simulation
├── UI/
│   ├── Controls/
│   │   ├── ModernTheme.cs        # Modern dark theme color tokens & geometry utilities
│   │   ├── ModernButton.cs       # Custom painted rounded button with hover/active states
│   │   ├── ModernProgressBar.cs  # Smooth gradient progress bar with percentage readout
│   │   ├── ModernCard.cs         # Double-buffered rounded container card
│   │   └── ModernBadge.cs        # Status pill badge with pulsing indicator dot
│   └── UpdateDialogForm.cs       # The cool update modal (changelog, progress, forced update)
├── Form1.cs                      # Main diagnostic dashboard & simulation studio
├── Form1.Designer.cs
├── Program.cs                    # VelopackApp entry point
└── WinFormsApp1.csproj           # Assembly: VelopackUpdateTest (.NET 9.0 Windows)
.github/
└── workflows/
    ├── build-and-release.yml     # Automated Velopack pack & GitHub Release workflow
    └── ci.yml                    # Push & PR compilation validation
```

---

## 🚀 Getting Started

### Prerequisites
- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) (or .NET 10 preview SDK)
- Windows 10/11 x64

### Running Locally
```powershell
# Restore and run the application
dotnet run --project WinFormsApp1/WinFormsApp1.csproj
```

When running in Visual Studio or via `dotnet run`, the app detects that it is in **Development Mode** (`UpdateManager.IsInstalled == false`) and automatically enables the **Simulation Sandbox** so you can test all UI workflows immediately!

---

## 🧪 Testing the Simulation Modes

1. Launch `VelopackUpdateTest`.
2. Look at the **Update Simulation Studio** on the left:
   - **Preset 1 (v1.1.0 - Feature Pack)**: Click **🚀 Launch Cool Update Screen (Simulation)**. You'll see a clean, optional update modal where you can click "Update Now" or "Remind Me Later".
   - **Preset 2 (v2.0.0 - Mandatory Force Update)**: Notice the red warning banner and locked "Skip (Locked)" button. You must click "Install Update Now" to proceed. If you try to close the window, it warns that the update is required and gives the option to exit the application.
   - **Simulate Network Error**: Check "Simulate Network Interruption / Drop at 50%" and start the update. At 50%, a simulated connection drop triggers and shows error recovery options!
   - **Top Notification Banner**: Click "🔔 Test Top In-App Update Banner" to test the in-app slide-down alert.

---

## 📦 Setting Up Real GitHub Releases

To deploy real updates to your users through GitHub:

1. Push this repository to GitHub:
   ```bash
   git init
   git add .
   git commit -m "feat: initial commit of VelopackUpdateTest"
   git remote add origin https://github.com/YOUR_USERNAME/VelopackUpdateTest.git
   git push -u origin main
   ```

2. Enter your repository URL in the **GitHub Feed Source** textbox in `Form1.cs` (or hardcode it in `VelopackUpdateService.cs`).

3. Create a new release automatically using GitHub Actions:
   - Go to **Actions** ➔ **Build and Release (Velopack)** ➔ Click **Run workflow**.
   - Input version `1.1.0`.
   - The workflow compiles the app, runs `vpk pack`, and publishes the release with `VelopackUpdateTest-Setup.exe` and `RELEASES` assets!
   
4. Install the generated `VelopackUpdateTest-Setup.exe` on your machine.
   - Now when you launch the installed app, `UpdateManager.IsInstalled` will be `true`, and clicking "Check Updates" will fetch real updates from GitHub!

---

## 📜 License
MIT License. Created for Velopack testing & simulation.
