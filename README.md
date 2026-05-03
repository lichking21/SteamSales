# Steam Deals Aggregator CLI

A high-performance, asynchronous Command Line Interface (CLI) application built with .NET for tracking Steam game prices and discounts. Developed as part of a university project demonstrating Clean Architecture, SOLID principles, and advanced asynchronous I/O operations.

## 🚀 Key Features

* **Concurrent HTTP Fetching:** Utilizes `Task.WhenAll` for parallel asynchronous calls to the Steam API, drastically reducing I/O bottlenecks when processing large wishlists.
* **Optimized In-Memory Searching:** Parses up to 250k+ local JSON records into a `Dictionary<string, long>`, enabling $O(1)$ lookup complexity for game titles.
* **Fuzzy Matching & Normalization:** Automatically strips trademarks (™, ®, ©) from inputs and database keys to ensure robust, case-insensitive partial matching.
* **Robust Fault Tolerance:** Implements custom exceptions (`SteamApiException`) and global try-catch blocks to gracefully handle network timeouts and JSON deserialization failures without crashing the runtime.
* **Clean Architecture:** Strict separation of concerns across `Domain`, `Application`, and `Infrastructure` layers, leveraging Dependency Injection (DI) for loose coupling.

## 🏗️ Architecture Stack

* **Platform:** .NET 10
* **Libraries:** `Microsoft.Extensions.Logging`, `System.Text.Json`
* **Design Patterns:** Dependency Injection, Inversion of Control (IoC), Repository-like Data Access.

## ⚙️ Installation & Setup

1.  **Clone the repository:**
    ```bash
    git clone [https://github.com/yourusername/steam-deals-cli.git](https://github.com/yourusername/steam-deals-cli.git)
    cd steam-deals-cli
    ```
2.  **Provide the Data Dumps:**
    Download the official Steam AppList JSONs and place them in the CLI directory with folder name `/GamesLists/` (name them `List1.json` through `List5.json`).
    They can be found at the main branch in CLI folder (it was made to avoid using of neccessary Steam API key)
3.  **Run the application:**
    ```bash
    dotnet run
    ```

## 🎮 Usage

Upon launch, the application will hydrate the in-memory dictionary. Follow the prompt to set your regional pricing (e.g., `us`, `kz`, `tr`).

**Available Commands in the UI loop:**
* `[title]` - Type any game name (e.g., "Cyberpunk 2077") to add it to your wishlist queue.
* `[l]` or `[list]` - Fire concurrent API requests and display sorted prices & discounts.
* `[e]` or `[enter]` - Execute final price fetch and exit the input loop.
* `[q]` or `[quit]` - Terminate the application.
