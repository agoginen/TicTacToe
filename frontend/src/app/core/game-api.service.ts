import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { GameMode, GameState, Player, Scoreboard } from './models';

// Backend base URL for local development (see backend/TicTacToe.Api/Properties/launchSettings.json).
const API_BASE_URL = 'http://localhost:5054/api';

@Injectable({ providedIn: 'root' })
export class GameApiService {
  private readonly http = inject(HttpClient);

  createGame(mode: GameMode): Observable<GameState> {
    return this.http.post<GameState>(`${API_BASE_URL}/games`, { mode });
  }

  getGame(gameId: string): Observable<GameState> {
    return this.http.get<GameState>(`${API_BASE_URL}/games/${gameId}`);
  }

  submitMove(gameId: string, player: Player, row: number, col: number): Observable<GameState> {
    return this.http.post<GameState>(`${API_BASE_URL}/games/${gameId}/moves`, { player, row, col });
  }

  undo(gameId: string): Observable<GameState> {
    return this.http.post<GameState>(`${API_BASE_URL}/games/${gameId}/undo`, {});
  }

  resetGame(gameId: string): Observable<GameState> {
    return this.http.post<GameState>(`${API_BASE_URL}/games/${gameId}/reset`, {});
  }

  getScoreboard(): Observable<Scoreboard> {
    return this.http.get<Scoreboard>(`${API_BASE_URL}/scoreboard`);
  }

  resetScoreboard(): Observable<Scoreboard> {
    return this.http.post<Scoreboard>(`${API_BASE_URL}/scoreboard/reset`, {});
  }
}
