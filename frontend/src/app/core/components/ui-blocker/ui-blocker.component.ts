import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { LoadingService } from '../../services/loading.service';

@Component({
  selector: 'app-ui-blocker',
  standalone: true,
  imports: [CommonModule, MatProgressSpinnerModule],
  templateUrl: './ui-blocker.component.html',
  styleUrl: './ui-blocker.component.css'
})
export class UiBlockerComponent {
  loadingService = inject(LoadingService);

  get isBlocked(): boolean {
    return this.loadingService.isBlocked();
  }
}
