# 🏛️ Municipal Services App

A municipal web application built with **ASP.NET Core MVC** and **Entity Framework**, allowing users to view local **events**, submit **community reports**, and explore smart **recommendations** based on interests and searches.

---

## 📘 Overview

The **Municipal Services App** helps citizens stay informed and connected with their municipality.  
It allows users to:
- Discover upcoming **events** and community announcements  
- Submit **reports** for public issues (e.g., damaged roads, water leaks, etc.)  
- Get **personalized event recommendations** based on recent searches and interests  

The app uses efficient **data structures** like queues, stacks, and dictionaries to manage performance and search speed.

---

## ⚙️ Main Features

### 🔹 Event Management
Users can:
- Add new events  
- Search and filter events by **category**, **date**, or **keywords**  
- View recommended events generated from recent search activity  
- Automatically organize events by date and category for quick access  

---

### 🔹 Report Submission (User Function)
Citizens can submit a report containing:
- Name of the reporter  
- Report category (e.g., Water, Road, Electricity, etc.)  
- Description of the issue  

Once submitted, reports are saved to the database and can be reviewed by administrators.

---

### 🔹 Search and Recommendation System
- The system records and tracks recent searches to build recommendations  
- Uses **priority queues** to identify upcoming events  
- Suggests events related to user interest or top-searched categories  
- Displays recommendations dynamically at the bottom of the interface  

---

### 🔹 Undo and Rebuild System
- Every modification (add/remove) is tracked using a **stack-based undo mechanism**  
- Indexes are dynamically rebuilt for accuracy when events are removed  

---

## 🧩 Core Structures Used

- **`SortedDictionary<DateTime, List<Event>>`** → Groups events by date  
- **`Dictionary<string, List<Event>>`** → Stores events by category  
- **`PriorityQueue<Event, DateTime>`** → Keeps track of upcoming events  
- **`Queue<string>`** → Stores recent searches  
- **`Stack<Action>`** → Supports undo operations  
- **`HashSet<string>`** → Keeps all categories unique  

---

## 🧠 How It Works (Summary)

1. When the app starts, all events are loaded from the database and indexed.  
2. The user can **add**, **search**, or **filter** events.  
3. The system saves **recent searches** to recommend future events.  
4. Users can **submit reports** describing issues in their area.  
5. Admins can manage both events and reports from the dashboard.  

---

## 🗂️ Project Structure
Municipal_services_app/
│
├── Controllers/
│ ├── EventController.cs
│ └── ReportController.cs
│ └── HomeController.cs
│
├── Models/
│ ├── Announcement.cs
│ └── AppDbContext.cs
│ └── ErrorViewModel.cs
│ └── Event.cs
│ └── EventsIndexViewModel.cs
│ └── SearchTerm.cs
│ └── Seeder.cs
│ 
├── Services/
│ └── EventStore.cs
│
├── Views/
│ ├── Event/
│ └── Report/
│ └── Home/
│
└── README.md

 ---
 
## 🧱 Technologies Used

<p align="center">
  <img src="https://skillicons.dev/icons?i=cs,dotnet,sqlite,bootstrap,html,css,js,visualstudio" />
</p>

- **C#** / **ASP.NET Core MVC**  
- **Entity Framework Core (EF Core)**  
- **LINQ** for data querying  
- **SQL Server / SQLite (for local testing)**  
- **Bootstrap** for front-end styling  
- **JavaScript** for interactivity  
- **Visual Studio / VS Code** for development  

---

## 🚀 Getting Started

1. Clone the repository  
   ```bash
   git clone https://github.com/stxphanx30/Municipal_services_app.git
   cd Municipal_services_app

2. Update your connection string in appsettings.json or leave the default one

3. Run the database migration:
   
**update-database** in the package manager console
Start the project:
**dotnet run**
Open your browser at:
**👉 https://localhost:5001**

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
| Method   | Endpoint          | Description                                             |
| :------- | :---------------- | :------------------------------------------------------ |
| **POST** | `/Report/Create` | Submit a new report (name, category, description, image) |

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

🧠 Example Flow

1. User searches for “Youth Events”.

2. Search term is added to the Queue and stored in searchCounts.

3. Recommendations update automatically based on frequency.

4. Events are displayed using SortedDictionary (by date) and Dictionary (by category).

5. Reports can be submitted anytime using the ReportController.

--- 

📄 License

This project is open-source under the MIT License.

---

👨‍💻 Author

Developed by Stéphane Kibambo
Municipal Services App © 2025





