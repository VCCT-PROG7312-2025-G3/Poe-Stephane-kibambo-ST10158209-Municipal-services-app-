# 🏙 Municipal Services Web Application

## 📖 Project Description

The *Municipal Services App* is a comprehensive ASP.NET MVC web platform designed for residents and municipal administrators to interact efficiently.  
It provides an all-in-one solution for *reporting local issues, **tracking service requests, **viewing local events, and **reading announcements* — promoting community transparency and smart city governance.

Built with *ASP.NET Core MVC, **Entity Framework Core, and **Bootstrap 5, this system leverages **advanced data structures* — such as *Binary Search Trees (BSTs), **Heaps, and **Graphs* — to organize, manage, and optimize service request tracking.

---
🤖 AI Disclosure & Assistance

This project’s development included occasional AI assistance for:

Code refactoring and explanation of data structures (BST, Heap, Graph).

Guidance on secure authorization and MVC best practices.

UI/UX design improvements (Bootstrap-based layouts and color harmonization).

The AI tools were used for learning, optimization, and documentation support only —
all code was reviewed, customized, and tested by the author

---

## 🚀 Features

### 👥 Citizen Features
- 📝 *Report Issues:* Submit detailed service requests with descriptions, categories, and file attachments.  
- 🔎 *Track Service Requests:* Follow your request using a unique *Request Reference ID*, with live status updates.  
- 📅 *Local Events & Announcements:* View and filter community events and municipal news.  
- 🧠 *Smart Recommendations:* Get event recommendations based on recent searches and interactions.  
- 📂 *Category & Date Filters:* Filter content dynamically to find relevant information quickly.

### 🧑‍💼 Admin Features
- 🔐 *Secure Admin Login:* Username & password authentication.  
- 📊 *Admin Dashboard:* Review, search, and filter all service requests.  
- ✏ *Update Request Status:* Change request states (Submitted → In Progress → Resolved → Closed).  
- 📥 *File Download:* View or download user-submitted attachments.  
- 🚪 *Logout Option:* End admin session securely.  

---

## 🧠 Technologies Used

<p align="left">
  <img src="https://skillicons.dev/icons?i=dotnet,cs,bootstrap,javascript,html,css,sqlite,git,github,vscode" />
</p>

- *Frontend:* HTML5, CSS3, Bootstrap 5, JavaScript  
- *Backend:* ASP.NET Core MVC (C#)  
- *Database:* Entity Framework Core with SQLite  
- *Architecture:* MVC with layered services (Controllers, Models, Services)  
- *IDE:* Visual Studio 2022 / VS Code  
- *Version Control:* Git & GitHub  

---

## ⚙ How It Works

### 🧾 Service Request Tracking (Part 3)
- Each report is assigned a unique *Request Reference (MS-xxxx)*.  
- A *Binary Search Tree (BST)* is used for fast lookup by reference.  
- A *Priority Queue* (min-heap) prioritizes older requests for admin attention.  
- A *Graph Structure* links related issues by location.  
- A *Dictionary* organizes requests by category for quick filtering.  
- Admins can update statuses, and history logs are recorded for transparency.

### 📅 Local Events & Announcements
- Events and announcements are loaded from the database and displayed with dynamic filters.
- Users can search and filter by *keyword, **category, or **date range*.
- A smart recommendation engine analyzes recent searches and suggests similar events.

### 🔐 Authentication
- Secure *Admin Login* using credentials stored in configuration (appsettings.json).
- Unauthorized users are redirected to an *Access Denied* view.
- Session-based authentication ensures controlled admin access.

---



## 🗂️ Project Structure
```
Municipal_services_app/
|
├── Controllers/
|   ├── HomeController.cs
|   ├── ReportController.cs
|   ├── EventController.cs
|   ├── RequestController.cs
|   ├── AdminRequestsController.cs
|   └── AccountController.cs
|
├── Models/
|   ├── Announcement.cs
|   ├── Event.cs
|   ├── Issue.cs
|   ├── Recommendation.cs
|   ├── AppDbContext.cs
|   └── ViewModels (for admin & requests)
|
├── Services/
|   ├── EventStore.cs
|   └── RequestStore.cs   ← BST, Heap, Graph logic here
|
├── Views/
|   ├── Shared/ (_Layout.cshtml, _ValidationScriptsPartial.cshtml)
|   ├── Event/
|   ├── Report/
|   ├── Request/
|   ├── AdminRequests/
|   └── Account/
|
├── wwwroot/
|   ├── css/
|   ├── js/
|   └── svg/ (municipal branding)
|
├── appsettings.json
└── README.md 
```

 ---

## 🚀 Getting Started

1. Clone the repository  
   ```bash
   git clone https://github.com/stxphanx30/poe-part-2-Stephane-kibambo-ST10158209-Municipal-services-app-.git

2. Update your connection string in appsettings.json or leave the default one

3. In the package manager console, run the database migration:
   ```bash
   update-database 

4. Start the project:
   ```bash
   dotnet run

---

📡 API Endpoints 
🗓️ Event Endpoints
| Method   | Endpoint                                   | Description                    |
| :------- | :----------------------------------------- | :----------------------------- |
| **GET**  | `/Events`                                  | Display all events             |
| **GET**  | `/Events/Details/{id}`                     | Show event details             |
| **GET**  | `/Events/Search?text=music&category=Youth` | Search events by text/category |
| **GET**  | `/Events/Recommend`                        | Get recommended events         |

---

📝 Report Endpoints
| Method   | Endpoint                 | Description                         |
| -------- | ------------------------ | ----------------------------------- |
| **POST** | `/Report/Create`         | Submit a new report                 |
| **GET**  | `/Request/Status`        | View all submitted service requests |
| **GET**  | `/Request/Details/{ref}` | View details and status timeline    |


---

🧠 Admin Actions
| Method   | Endpoint                    | Description                         |
| -------- | --------------------------- | ----------------------------------- |
| **GET**  | `/AdminRequests/Index`      | View all requests (Admin only)      |
| **GET**  | `/AdminRequests/Edit/{ref}` | Update status of a specific request |
| **POST** | `/Account/Login`            | Authenticate as admin               |
| **POST** | `/Account/Logout`           | End admin session                   |

---

🧮 Data Structure Responsibilities
| Structure                                 | Purpose                    |
| :---------------------------------------- | :------------------------- |
| `Queue<string>`                           | Stores recent search terms |
| `PriorityQueue<Event, DateTime>`          | Manages upcoming events    |
| `Stack<Action>`                           | Handles undo operations    |
| `Dictionary<string, List<Event>>`         | Categorizes events         |
| `HashSet<string>`                         | Keeps categories unique    |
| `SortedDictionary<DateTime, List<Event>>` | Groups events by date      |

---

🧠 Example Workflow

1️⃣ A user reports a road issue with an attachment.
2️⃣ The issue is stored in the DB and indexed in a BST for quick reference.
3️⃣ An admin logs in and views it on the dashboard via a heap-prioritized queue.
4️⃣ The admin updates the status to “In Progress.”
5️⃣ The user can check the current status using the request reference code.

--- 

📄 License

This project is open-source under the MIT License.

---

👨‍💻 Author

Developed by Stéphane Kibambo
Municipal Services App © 2025





