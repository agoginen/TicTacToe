import { Component, input, output } from '@angular/core';
import { GameMode } from '../../core/models';

@Component({
  selector: 'app-mode-selector',
  templateUrl: './mode-selector.html',
  styleUrl: './mode-selector.css',
})
export class ModeSelector {
  readonly mode = input.required<GameMode>();
  readonly modeChange = output<GameMode>();

  select(mode: GameMode): void {
    if (mode !== this.mode()) {
      this.modeChange.emit(mode);
    }
  }
}
