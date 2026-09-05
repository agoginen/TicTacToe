import { Component, input } from '@angular/core';
import { MoveHistoryEntry } from '../../core/models';

@Component({
  selector: 'app-move-history',
  templateUrl: './move-history.html',
  styleUrl: './move-history.css',
})
export class MoveHistory {
  readonly moves = input.required<MoveHistoryEntry[]>();

  position(move: MoveHistoryEntry): string {
    return `Row ${move.row + 1}, Column ${move.col + 1}`;
  }
}
