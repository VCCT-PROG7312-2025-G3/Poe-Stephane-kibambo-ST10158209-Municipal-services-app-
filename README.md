# 🏙️ Municipal Services App

## 📖 Project Description

The **Municipal Services App** is a modern web platform designed to help municipalities efficiently manage and share local information.  
It provides a centralized system for displaying **events**, **announcements**, and **community reports**, allowing citizens to stay informed and engaged with their local government.

Built with **ASP.NET Core MVC** and **Entity Framework Core**, the app utilizes advanced data structures, including **queues**, **stacks**, **dictionaries**, and **priority queues**, to optimize search, categorization, and recommendation performance.  

Users can:
- Browse and search for upcoming **events**
- View official **announcements**
- Submit **reports** for municipal issues (e.g., road, water, or electricity problems)
- Receive **personalized recommendations** based on recent search behavior

The platform is fully modular and scalable, making it suitable for integration into larger smart city systems.

---

## 🚀 Features
- 📅 Event Management(View, Search, Filter)
- 📢 Announcements Section
- 🧠 Smart Recommendations based on user searches
- 🗂️ Categorization by type and date
- 📝 Report Submission (category, description, location)
- 🧱 Efficient data handling using in-memory structures
- 🧭 Undo stack and queue-based search tracking
- ⚡ Persistent recommendation data (saved in database)

---

## 🧠 Technologies Used

<p align="left">
  <img src="https://skillicons.dev/icons?i=dotnet,cs,bootstrap,javascript,html,css,sqlite,git,github,vscode" />
</p>

- **Frontend:** HTML5, CSS3, Bootstrap, JavaScript (Dynamic filtering and search)
- **Backend:** ASP.NET Core MVC (C#)
- **Database:** Entity Framework Core (SQLite / SQL Server)
- **Version Control:** Git & GitHub
- **IDE:** Visual Studio / VS Code

---

## ⚙️ How It Works

### 🔍 Search & Recommendation Logic
- When a user types a search query, it’s stored in a **queue**.
- The system tracks the most frequent search terms in a **dictionary**.
- Using a **priority queue**, the app identifies and recommends events most relevant to the user’s interests.
- Recommendations are **persisted** to the database so that they remain after page reload or restart.

### 🧾 Report Submission
Users can submit reports to the municipality directly through the platform.

Each report includes:
- Reporter name  
- Category (e.g., “Water Issue”, “Road Damage”)  
- Description of the problem  
- Optional photo or document  

The data is stored in the database and can be viewed by municipal staff for follow-up.

---

## 🗂️ Project Structure
```
Municipal_services_app/
|
├── Controllers/
|   ├── EventController.cs
|   ├── ReportController.cs
|   └── HomeController.cs
|
├── Models/
|   ├── Announcement.cs
|   ├── AppDbContext.cs
|   ├── ErrorViewModel.cs
|   ├── Event.cs
|   ├── EventsIndexViewModel.cs
|   ├── SearchTerm.cs
|   └── Seeder.cs
|
├── Services/
|   └── EventStore.cs
|
├── Views/
|   ├── Event/
|   ├── Report/
|   └── Home/
|
└── README.md
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





