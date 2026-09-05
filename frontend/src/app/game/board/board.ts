import { Component, computed, input, output } from '@angular/core';
import { Player } from '../../core/models';

@Component({
  selector: 'app-board',
  templateUrl: './board.html',
  styleUrl: './board.css',
})
export class Board {
  readonly board = input.required<(Player | null)[]>();
  readonly winningCells = input<number[] | null>(null);
  readonly disabled = input(false);

  readonly cellClick = output<{ row: number; col: number }>();

  readonly winningSet = computed(() => new Set(this.winningCells() ?? []));

  onCellClick(row: number, col: number): void {
    if (this.disabled() || this.board()[row * 3 + col] !== null) {
      return;
    }

    this.cellClick.emit({ row, col });
  }

  isWinningCell(index: number): boolean {
    return this.winningSet().has(index);
  }
}
