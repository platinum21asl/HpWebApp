# 🏢 Herta Enterprise Web Application (N-Tier Architecture)

![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)
![ASP.NET](https://img.shields.io/badge/ASP.NET_Core-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![N-Tier Architecture](https://img.shields.io/badge/Architecture-N--Tier-0052CC?style=for-the-badge)

## 📌 Project Description
**Herta Enterprise Web App** is a robust web application built to demonstrate the implementation of a strict **N-Tier Architecture** using **C#** and **ASP.NET**. This project was designed with a strong focus on *Separation of Concerns (SoC)*, ensuring that the presentation, business logic, data access, and common utilities are completely decoupled.

Initially started as an experimental/testing ground, it has been structured to serve as a foundational boilerplate for scalable, enterprise-level web development.

## 🏛️ Architectural Structure
The solution is divided into distinct, decoupled projects to maintain code clarity and scalability:

- 🖥️ **`HertaProject` (Presentation Layer):** The main entry point of the application handling user interface, routing, and HTTP requests (MVC).
- ⚙️ **`HertaProjectDataAccess` (Data Access Layer):** Responsible for all database interactions, query executions, and data retrieval processes.
- 📦 **`HertaProjectModels` (Domain/Entity Layer):** Contains data models, view models, and Data Transfer Objects (DTOs) used across the application.
- 🛠️ **`HertaProjectUtility` (Utility/Helper Layer):** A shared library housing common functions, constants, security helpers, and extensions.
- 📄 **`HertaProjectRazor_Temp`:** Secondary/Prototype UI project utilizing ASP.NET Razor Pages for rapid page-based development.

## 🛠️ Tech Stack
- **Backend:** C# (.NET)
- **Web Framework:** ASP.NET (MVC / Razor Pages)
- **Frontend:** HTML5, CSS3, JavaScript
- **Architecture Pattern:** N-Tier / Multi-Tier Layered Architecture

## 🚀 Getting Started

### Prerequisites
- [.NET SDK](https://dotnet.microsoft.com/download)
- Visual Studio 2022 (Recommended) or Visual Studio Code
- SQL Server (If local database mapping is configured)

### Installation & Execution
1. **Clone the repository:**
   ```bash
   git clone [https://github.com/platinum21asl/HpWebApp.git](https://github.com/platinum21asl/HpWebApp.git)

2. Open the HertaProject.sln file using Visual Studio.
3. Rebuild the solution to restore all necessary NuGet packages across the projects.
4. Ensure your database connection strings (if any) inside the presentation layer's configuration files (e.g., appsettings.json or Web.config) are pointing to your local environment.
5. Set HertaProject as the Startup Project and press F5 to run the application in debug mode.

Developed by Daniel Renato M as a showcase of structured, scalable software engineering practices.
