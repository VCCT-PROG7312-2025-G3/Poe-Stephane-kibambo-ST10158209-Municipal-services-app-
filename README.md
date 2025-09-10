# 🏛️ Municipal Services App

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet&logoColor=white)
![ASP.NET Core MVC](https://img.shields.io/badge/ASP.NET%20Core-MVC-5C2D91?logo=dotnet&logoColor=white)
![EF Core](https://img.shields.io/badge/Entity%20Framework%20Core-8-512BD4?logo=dotnet&logoColor=white)
![SQLite](https://img.shields.io/badge/SQLite-DB-003B57?logo=sqlite&logoColor=white)
![Bootstrap 5](https://img.shields.io/badge/Bootstrap-5-7952B3?logo=bootstrap&logoColor=white)
![Visual Studio](https://img.shields.io/badge/Visual%20Studio-2022-5C2D91?logo=visualstudio&logoColor=white)
![VS Code](https://img.shields.io/badge/Visual%20Studio%20Code-Ready-007ACC?logo=visualstudiocode&logoColor=white)
![Windows](https://img.shields.io/badge/Windows-Supported-0078D6?logo=windows&logoColor=white)
![macOS](https://img.shields.io/badge/macOS-Supported-000000?logo=apple&logoColor=white)
![Linux](https://img.shields.io/badge/Linux-Supported-FCC624?logo=linux&logoColor=000)

A resident-friendly web app for **reporting municipal issues** (location, category, description, optional attachment), designed for inclusion, low data usage, and trust. Built with ASP.NET Core MVC (.NET 8), EF Core (SQLite), and Bootstrap 5.

---

## 📚 Table of Contents

- [What this app does](#-what-this-app-does)
- [Tech stack](#-tech-stack)
- [Screens & flow](#-screens--flow)
- [Data model](#-data-model)
- [Storage locations](#-storage-locations)
- [Validation & error handling](#-validation--error-handling)
- [Accessibility & engagement](#-accessibility--engagement)
- [Security & privacy](#-security--privacy)
- [How to run (high level)](#-how-to-run-high-level)
- [Troubleshooting (high level)](#-troubleshooting-high-level)
- [Roadmap / next steps](#-roadmap--next-steps)
- [License](#-license)

---

## 🧭 What this app does

- **Report Issues**: Residents submit a location, select a category (e.g., sanitation, roads, utilities), add a description, and optionally attach an image/PDF.
- **Engagement UX**: Live progress indicator with friendly hints and checkmarks as fields are completed.
- **Confirmation**: A success screen provides a **reference number** for follow-up.
- **Admin-style views (optional)**: List and detail pages to browse submissions and securely download attachments.
- **Resilient UX**: Clear validation messages, disabled submit until valid, and graceful error/status pages (404/403/etc.).

---

## 🧩 Tech stack

- **ASP.NET Core MVC (.NET 8)** — routing, controllers, Razor views.
- **Entity Framework Core** — data access with migrations.
- **SQLite** — lightweight, file-based database (no server required).
- **Bootstrap 5 + Font Awesome** — responsive layout and icons.
- **jQuery Validate + Unobtrusive** — client-side validation.

IDE support:
- **Visual Studio 2022** — full-featured .NET IDE.
- **Visual Studio Code** — lightweight editor with C# support.

---

## 📱 Screens & flow

1. **Home / Landing**  
   Hero section, quick explanation, and three cards:
   - **Report Issues** (active),
   - **Local Events** (coming soon),
   - **Service Request Status** (coming soon).

2. **Report Issues (Create)**  
   Fields for Location, Category (dropdown), Description, and optional Attachment.  
   A side panel shows an **engagement progress bar**, hints, and completion ticks.

3. **Success**  
   Displays a **Reference #** with key details and actions to submit another or return home.

4. **(Optional) List & Details**  
   A simple table of issues and a detail page with a **secure download** link for attachments.

---

## 🧱 Data model

**Issue**
- `Id` — numeric reference number.
- `Location` — text (e.g., street + ward).
- `Category` — sanitation, roads, utilities, water, electricity, parks, etc.
- `Description` — resident’s notes about the issue.
- `AttachmentPath` — relative path to the uploaded file (if any).
- `CreatedAt` — timestamp (UTC).
- `Status` — default “Submitted”.

**IssueCreateVm** (form view model)
- Mirrors the fields needed on the create form and enforces required entries via validation attributes.

---

## 💽 Storage locations

- **Database (SQLite)**: `App_Data/municipal.db`  
  Created by EF Core migrations.

- **Uploads (attachments)**: `App_Data/Uploads/`  
  Files are saved **outside** the public `wwwroot` and streamed via a controller action for security.

> These folders live in the project directory. Because SQLite is file-based, avoid opening the DB file in an external viewer while the app is running.

---

## ✅ Validation & error handling

**Client-side**
- Required fields, max lengths, and formats enforced via **jQuery Validate + Unobtrusive**.
- The **Submit** button stays disabled until the form is valid.
- File input shows **always-grey** styling and pre-checks **type** (image/PDF) and **size** (≤ 8 MB) with inline messages and optional toast.

**Server-side**
- DataAnnotations enforce required fields and limits.
- The upload pipeline re-validates file type and size **before** saving.

**Errors**
- A global error page handles unexpected exceptions in production.
- Status code pages reroute to a friendly screen (e.g., 404 “Page not found”).

---

## ♿ Accessibility & engagement

- **Clear labels and plain language**; responsive layout for mobile and desktop.
- **Skip-to-content** link and ARIA roles/hints where appropriate.
- **Engagement panel** that motivates completion and clarifies next steps.
- Inclusive defaults, with the intent to extend to multiple languages and low-data channels.

---

## 🔐 Security & privacy

- **Uploads are not publicly served**: files live under `App_Data/Uploads` and are only accessible via a controller action that checks paths and content types.
- **File validation**: whitelisted extensions (images/PDF) and strict max size.
- **Reference numbers** avoid exposing personal details in URLs.
- Recommended for production: security headers (CSP, X-Content-Type-Options), HTTPS, rate limiting, and access control for admin pages.

---

## 🚀 How to run (high level)

- Clone or download the project.
- Add EF Core packages for **SQLite** and **design/migrations** should be installed already.
- Open the package manager console, then run **Update-database** should be installed already.
- Start the app, then navigate to **Report Issues** and submit a test entry.

---

## 🩺 Troubleshooting (high level)

- **Can’t open the DB file**: ensure migrations have been run; the file shouldn’t be 0 bytes. If using a viewer, close the running app or open a **copy** of the file.
- **404 on Report Issues**: make sure the create view exists and the default route maps to `Home/Index`.
- **Uploads not accessible via URL**: expected; attachments are not served directly. Use the secure download action.
- **Validation isn’t triggering**: ensure the client-side validation scripts are included after the form and that fields have validation attributes.

---

## 🗺️ Roadmap / next steps

- Internationalization (e.g., en/af/zu/xh).
- “Service Request Status” lookup by reference.
- Ward selection and geo-coordinates.
- Admin authentication and dashboards (SLAs, heatmaps).
- Production security headers and telemetry.
- Automated tests (unit/integration).

---

### 👩🏽‍💻 Built with

![Visual Studio](https://img.shields.io/badge/Visual%20Studio-2022-5C2D91?logo=visualstudio&logoColor=white)
![VS Code](https://img.shields.io/badge/Visual%20Studio%20Code-007ACC?logo=visualstudiocode&logoColor=white)
![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet&logoColor=white)
![ASP.NET Core MVC](https://img.shields.io/badge/ASP.NET%20Core-MVC-5C2D91?logo=dotnet&logoColor=white)
![EF Core](https://img.shields.io/badge/Entity%20Framework%20Core-8-512BD4?logo=dotnet&logoColor=white)
![SQLite](https://img.shields.io/badge/SQLite-003B57?logo=sqlite&logoColor=white)
![Bootstrap 5](https://img.shields.io/badge/Bootstrap-5-7952B3?logo=bootstrap&logoColor=white)







