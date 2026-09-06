# Tic Tac Toe

A full-stack Tic Tac Toe application with a .NET Web API backend and an Angular frontend. Supports two-player (human vs human) and single-player (human vs computer) modes, move history, undo, and a persistent scoreboard.

## Tech Stack

| Backend  | ASP.NET Core Web API (.NET 10), Swagger/OpenAPI, xUnit    |
| Frontend | Angular 22 (standalone components, signals), Vitest      |

## Project Structure

```
TicTacToe/
├── backend/
│   ├── TicTacToe.Api/          # ASP.NET Core Web API
│   │   ├── Controllers/        # GamesController, ScoreboardController
│   │   ├── Engine/             # GameEngine, BoardEvaluator, ComputerPlayer
│   │   ├── Models/              # Domain models (GameSession, Player, ...)
│   │   ├── Storage/             # In-memory session/scoreboard stores
│   │   └── Contracts/           # API request/response DTOs
│   └── TicTacToe.Api.Tests/     # xUnit test suite
└── frontend/
    └── src/app/
        ├── core/                # API client, app state, models
        └── game/                # Board, scoreboard, move history, controls, etc.
```

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/) (v20+) and npm
- [Angular CLI](https://angular.dev/tools/cli) (`npm install -g @angular/cli`) — optional, `npx ng` also works

## Getting Started

### 1. Run the backend API

```bash
cd backend/TicTacToe.Api
dotnet run
```

The API listens on:
- `http://localhost:5054`
- `https://localhost:7137`

Swagger UI is available at the root URL in Development (`/swagger`).

> **Note:** In Development, HTTPS redirection is disabled so the Angular dev server (running on plain HTTP) can call the API without CORS/redirect issues.

### 2. Run the frontend

```bash
cd frontend
npm install
npm start
```

Open `http://localhost:4200` in your browser. The app calls the backend at `http://localhost:5054/api`.

## Running Tests

**Backend:**
```bash
cd backend/TicTacToe.Api.Tests
dotnet test
```

**Frontend:**
```bash
cd frontend
npm test
```

## API Reference

Base URL: `/api`

| Method | Endpoint                     | Description                              |
| ------ | ----------------------------- | ----------------------------------------- |
| POST   | `/games`                      | Create a new game (`{ "mode": "TwoPlayer" \| "VsComputer" }`) |
| GET    | `/games/{id}`                 | Get the current state of a game           |
| POST   | `/games/{id}/moves`           | Submit a move (`{ "player", "row", "col" }`) |
| POST   | `/games/{id}/undo`            | Undo the last turn                        |
| POST   | `/games/{id}/reset`           | Reset the board (keeps the same game id)  |
| GET    | `/scoreboard`                 | Get win/draw totals                       |
| POST   | `/scoreboard/reset`           | Reset the scoreboard                      |

All responses share the `GameStateResponse` shape: `gameId`, `board`, `currentPlayer`, `mode`, `status`, `winner`, `winningCells`, `moveHistory`, and `scoreboard`.

## Game Rules & Policies

- **Undo:** Disabled once a game reaches `Won` or `Draw`. In `VsComputer` mode, undo reverts a full round (the player's move plus the computer's automatic reply) so play always returns to the human's turn.
- **Scoreboard:** Updated automatically when a game ends in a win or draw; entries are final once recorded.
- **Storage:** Game sessions and the scoreboard are stored in memory and reset when the API restarts.

## Development Notes

- CORS policy `AngularDev` allows `http://localhost:4200` and is registered before other middleware in `Program.cs`.
- The frontend's API base URL is configured in `frontend/src/app/core/game-api.service.ts`.
