import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { UserService } from '../../core/services/user.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule, MatCardModule, MatButtonModule, MatIconModule],
  template: `
    <div class="dashboard-container fade-in">
      <div class="welcome-banner">
        <div class="welcome-text">
          <h1>Asset Management Hub</h1>
          <p>Welcome back, <strong>{{ userService.userName() }}</strong> ({{ userService.userRole() }})</p>
        </div>
        <div class="session-badge">
          <span class="status-dot"></span>
          <span>Authenticated Session</span>
        </div>
      </div>

      <div class="cards-grid">
        <div class="info-card">
          <div class="card-icon cyan"><mat-icon>inventory_2</mat-icon></div>
          <div class="card-content">
            <h3>Total Assets</h3>
            <p class="stat-number">1,248</p>
            <span class="stat-meta">Active monitored devices</span>
          </div>
        </div>

        <div class="info-card clickable-card" routerLink="/forms">
          <div class="card-icon blue"><mat-icon>text_snippet</mat-icon></div>
          <div class="card-content">
            <h3>Form Components</h3>
            <p class="stat-text">Metadata Generator</p>
            <span class="stat-meta">Top & Left Labels • Live Value Stream</span>
          </div>
        </div>

        <div class="info-card">
          <div class="card-icon emerald"><mat-icon>verified_user</mat-icon></div>
          <div class="card-content">
            <h3>Active User Session</h3>
            <p class="stat-text">{{ userService.userEmail() }}</p>
            <span class="stat-meta">Role: {{ userService.userRole() | uppercase }}</span>
          </div>
        </div>

        <div class="info-card">
          <div class="card-icon purple"><mat-icon>key</mat-icon></div>
          <div class="card-content">
            <h3>JWT Expiration</h3>
            <p class="stat-text">{{ (userService.sessionInfo().expiresAt | date:'medium') ?? 'N/A' }}</p>
            <span class="stat-meta">Token: {{ (userService.jwtToken()?.substring(0, 20) ?? '') + '...' }}</span>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .dashboard-container {
      padding: 32px 24px;
      max-width: 1200px;
      margin: 0 auto;
    }
    .welcome-banner {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 28px 32px;
      background: linear-gradient(135deg, rgba(99, 102, 241, 0.15) 0%, rgba(6, 182, 212, 0.15) 100%);
      border: 1px solid rgba(99, 102, 241, 0.3);
      border-radius: var(--radius-lg, 20px);
      margin-bottom: 32px;
      backdrop-filter: blur(12px);
    }
    .welcome-text h1 {
      font-size: 1.8rem;
      font-weight: 800;
      margin: 0 0 6px 0;
      color: #ffffff;
    }
    .welcome-text p {
      color: var(--text-secondary, #94a3b8);
      margin: 0;
    }
    .session-badge {
      display: inline-flex;
      align-items: center;
      gap: 8px;
      padding: 8px 16px;
      background: rgba(16, 185, 129, 0.15);
      border: 1px solid rgba(16, 185, 129, 0.4);
      color: #34d399;
      border-radius: 20px;
      font-size: 0.85rem;
      font-weight: 600;
    }
    .status-dot {
      width: 8px;
      height: 8px;
      border-radius: 50%;
      background: #10b981;
      box-shadow: 0 0 10px #10b981;
    }
    .cards-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
      gap: 24px;
    }
    .info-card {
      padding: 24px;
      background: var(--bg-card, rgba(18, 24, 38, 0.75));
      border: 1px solid var(--border-color, rgba(255, 255, 255, 0.08));
      border-radius: var(--radius-md, 14px);
      display: flex;
      gap: 20px;
      align-items: center;
      transition: all 0.3s ease;
    }
    .info-card:hover {
      border-color: var(--border-highlight, rgba(99, 102, 241, 0.3));
      transform: translateY(-3px);
      box-shadow: var(--shadow-glow);
    }
    .card-icon {
      width: 54px;
      height: 54px;
      border-radius: 14px;
      display: flex;
      align-items: center;
      justify-content: center;
      flex-shrink: 0;
    }
    .card-icon mat-icon {
      font-size: 28px;
      width: 28px;
      height: 28px;
    }
    .info-card.clickable-card {
      cursor: pointer;
    }
    .card-icon.blue { background: rgba(37, 99, 235, 0.15); color: #3b82f6; }
    .card-icon.cyan { background: rgba(6, 182, 212, 0.15); color: #06b6d4; }
    .card-icon.emerald { background: rgba(16, 185, 129, 0.15); color: #10b981; }
    .card-icon.purple { background: rgba(168, 85, 247, 0.15); color: #a855f7; }
    .card-content h3 {
      font-size: 0.85rem;
      text-transform: uppercase;
      letter-spacing: 0.05em;
      color: var(--text-secondary, #94a3b8);
      margin: 0 0 6px 0;
    }
    .stat-number {
      font-size: 1.8rem;
      font-weight: 800;
      color: #ffffff;
      margin: 0;
    }
    .stat-text {
      font-size: 0.95rem;
      font-weight: 600;
      color: #ffffff;
      margin: 0;
      word-break: break-all;
    }
    .stat-meta {
      font-size: 0.775rem;
      color: var(--text-muted, #64748b);
    }
  `]
})
export class DashboardComponent {
  userService = inject(UserService);
}
