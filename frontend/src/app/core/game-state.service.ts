import { HttpErrorResponse } from '@angular/common/http';
import { Injectable, computed, inject, signal } from '@angular/core';
import { ApiProblem, GameMode, GameState } from './models';
import { GameApiService } from './game-api.service';

@Injectable({ providedIn: 'root' })
export class GameStateService {
  private readonly api = inject(GameApiService);

  readonly gameState = signal<GameState | null>(null);
  readonly errorMessage = signal<string | null>(null);

  // Undo policy: Option A - disabled once the game is Won/Draw (see backend GameEngine).
  readonly canUndo = computed(() => {
    const state = this.gameState();
    return !!state && state.status === 'InProgress' && state.moveHistory.length > 0;
  });

  readonly canPlay = computed(() => this.gameState()?.status === 'InProgress');

  startNewGame(mode: GameMode): void {
    this.errorMessage.set(null);
    this.api.createGame(mode).subscribe({
      next: (state) => this.gameState.set(state),
      error: (err) => this.handleError(err),
    });
  }

  play(row: number, col: number): void {
    const state = this.gameState();
    if (!state || state.status !== 'InProgress' || state.board[row * 3 + col] !== null) {
      return;
    }

    this.errorMessage.set(null);
    this.api.submitMove(state.gameId, state.currentPlayer, row, col).subscribe({
      next: (updated) => this.gameState.set(updated),
      error: (err) => this.handleError(err),
    });
  }

  undo(): void {
    const state = this.gameState();
    if (!state) {
      return;
    }

    this.errorMessage.set(null);
    this.api.undo(state.gameId).subscribe({
      next: (updated) => this.gameState.set(updated),
      error: (err) => this.handleError(err),
    });
  }

  resetGame(): void {
    const state = this.gameState();
    if (!state) {
      return;
    }

    this.errorMessage.set(null);
    this.api.resetGame(state.gameId).subscribe({
      next: (updated) => this.gameState.set(updated),
      error: (err) => this.handleError(err),
    });
  }

  resetScoreboard(): void {
    this.errorMessage.set(null);
    this.api.resetScoreboard().subscribe({
      next: (scoreboard) => {
        const current = this.gameState();
        if (current) {
          this.gameState.set({ ...current, scoreboard });
        }
      },
      error: (err) => this.handleError(err),
    });
  }

  private handleError(err: HttpErrorResponse): void {
    const problem = err.error as ApiProblem | undefined;
    this.errorMessage.set(problem?.detail ?? 'Something went wrong. Please try again.');
  }
}
