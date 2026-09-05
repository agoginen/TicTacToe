import { Component, input, output } from '@angular/core';

@Component({
  selector: 'app-game-controls',
  templateUrl: './game-controls.html',
  styleUrl: './game-controls.css',
})
export class GameControls {
  readonly canUndo = input.required<boolean>();

  readonly resetGame = output<void>();
  readonly undo = output<void>();
  readonly resetScoreboard = output<void>();
}
