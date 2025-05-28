# InSyURLCentral

A single-page ASP.NET Core Razor Pages web application that serves as a control panel for quick access to important URLs. The backend exposes a GET API to serve the list of URLs from `appsettings.json`. The frontend is a Progressive Web App (PWA) with install support and standalone launch mode.

## Features
- Control panel UI with clickable links and icons
- URLs and icons are configured in `appsettings.json`
- Backend API endpoint: `/api/ControlPanelLinks`
- PWA: installable on mobile/desktop, standalone mode, manifest.json, service worker
- Install prompt handling with a generic message

## Getting Started
1. **Run the app:**
   ```powershell
   dotnet run
   ```
2. **Access in browser:**
   Navigate to `https://localhost:5001` (or the port shown in the console)
3. **Install as PWA:**
   Use the "Install App" button or your browser's install option

## Configuration
- Edit `appsettings.json` to change the list of URLs and icons.

## Project Structure
- `Controllers/ControlPanelLinksController.cs`: API controller for URLs
- `Pages/Index.cshtml`: Main control panel UI
- `wwwroot/manifest.json`: PWA manifest
- `wwwroot/service-worker.js`: Service worker

---

© 2025 InSyURLCentral
