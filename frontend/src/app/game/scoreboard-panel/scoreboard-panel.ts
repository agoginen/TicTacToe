import { Component, input } from '@angular/core';
import { Scoreboard } from '../../core/models';

@Component({
  selector: 'app-scoreboard-panel',
  templateUrl: './scoreboard-panel.html',
  styleUrl: './scoreboard-panel.css',
})
export class ScoreboardPanel {
  readonly scoreboard = input.required<Scoreboard>();
}
