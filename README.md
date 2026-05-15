# Steam Deals Aggregator CLI

## Architecture & Design Rationale

* **Separation of Concerns:** The application is logically divided into the **Domain layer** (data models, DTOs), the **Application layer** (business logic — `WishlistManager`, interfaces), and the **Infrastructure layer** (HTTP communication, file handling). This strictly follows to the Single Responsibility Principle (SOLID).

* **Asynchronous Processing:** Network I/O operations (`HttpClient` in `PriceManager`) use `async/await` to avoid blocking the thread. Processing of multiple games occurs in parallel using `Task.WhenAll`, which drastically reduces latency when querying the Steam API.

* **Synchronous UI:** Reading user input via `Console.ReadLine()` remains synchronous, as this is a blocking I/O operation specific to console applications, where an asynchronous wrapper would offer no performance benefit.

* **Exceptions and Error States:** * Critical errors (e.g., missing JSON files) implement a "fail-fast" approach using exceptions. 
  * Communication with the API catches `HttpRequestException` and `JsonException`, which are transformed into the domain-specific `SteamApiException` and propagated to the UI layer for display to the user. 
  * User inputs (e.g., Region) are validated immediately upon entry.

## Stack

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
