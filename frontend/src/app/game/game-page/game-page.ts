import { Component, OnInit, inject } from '@angular/core';
import { GameStateService } from '../../core/game-state.service';
import { GameMode } from '../../core/models';
import { Board } from '../board/board';
import { StatusBar } from '../status-bar/status-bar';
import { ModeSelector } from '../mode-selector/mode-selector';
import { MoveHistory } from '../move-history/move-history';
import { ScoreboardPanel } from '../scoreboard-panel/scoreboard-panel';
import { GameControls } from '../game-controls/game-controls';

@Component({
  selector: 'app-game-page',
  imports: [Board, StatusBar, ModeSelector, MoveHistory, ScoreboardPanel, GameControls],
  templateUrl: './game-page.html',
  styleUrl: './game-page.css',
})
export class GamePage implements OnInit {
  protected readonly state = inject(GameStateService);

  ngOnInit(): void {
    this.state.startNewGame('TwoPlayer');
  }

  onModeChange(mode: GameMode): void {
    this.state.startNewGame(mode);
  }

  onCellClick(cell: { row: number; col: number }): void {
    this.state.play(cell.row, cell.col);
  }
}
