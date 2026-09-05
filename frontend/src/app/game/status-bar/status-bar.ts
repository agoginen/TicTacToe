import { Component, computed, input } from '@angular/core';
import { GameMode, GameStatus, Player } from '../../core/models';

@Component({
  selector: 'app-status-bar',
  templateUrl: './status-bar.html',
  styleUrl: './status-bar.css',
})
export class StatusBar {
  readonly currentPlayer = input.required<Player>();
  readonly mode = input.required<GameMode>();
  readonly status = input.required<GameStatus>();
  readonly winner = input<Player | null>(null);

  readonly modeLabel = computed(() => (this.mode() === 'VsComputer' ? 'Play Against Computer' : 'Two Player'));

  readonly message = computed(() => {
    if (this.status() === 'Won') {
      return `Player ${this.winner()} wins!`;
    }

    if (this.status() === 'Draw') {
      return "It's a draw!";
    }

    return `Player ${this.currentPlayer()}'s turn`;
  });
}
