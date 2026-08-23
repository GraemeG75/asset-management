import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatBadgeModule } from '@angular/material/badge';
import { MatChipsModule } from '@angular/material/chips';

import { UserService } from '../../core/services/user.service';
import { FormMetadataService } from '../../core/services/form-metadata.service';
import { UserBootstrapData, DashboardFormMetadata, ProfileNavLink, SiteNavLink } from '../../core/models/user-bootstrap.model';
import { FormSchema } from '../../core/models/form-schema.model';
import { FormComponent } from '../../core/components/form-controls/form.component';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatMenuModule,
    MatBadgeModule,
    MatChipsModule,
    FormComponent
  ],
  template: `
    <div class="dashboard-wrapper fade-in">
      <!-- 1. SITE NAVIGATION LINKS BAR -->
      <nav class="site-nav-bar">
        <div class="site-nav-links">
          @for (siteLink of siteLinks(); track siteLink.id) {
            <a 
              [routerLink]="siteLink.route" 
              class="site-nav-item"
              [class.active]="activeSiteTab() === siteLink.id"
              (click)="activeSiteTab.set(siteLink.id)"
            >
              <mat-icon class="nav-icon">{{ getMatIcon(siteLink.icon) }}</mat-icon>
              <span class="nav-label">{{ siteLink.label }}</span>
              @if (siteLink.badgeCount) {
                <span class="nav-count-badge">{{ siteLink.badgeCount }}</span>
              }
            </a>
          }
        </div>
      </nav>

      <!-- 3. INBOX / WELCOME BANNER -->
      <div class="inbox-banner-card">
        <div class="inbox-banner-content">
          <div class="inbox-icon-container">
            <mat-icon>mark_email_unread</mat-icon>
            @if (inboxCount() > 0) {
              <span class="inbox-pulsing-badge">{{ inboxCount() }}</span>
            }
          </div>
          <div class="inbox-text-content">
            <h2>User Inbox & Operational Hub</h2>
            <p>You have <strong>{{ inboxCount() }} pending action items</strong> requiring inspection, approval, or maintenance triage.</p>
          </div>
        </div>
        <div class="inbox-banner-actions">
          <button mat-raised-button class="quick-audit-btn" (click)="activeSiteTab.set('nav-dashboard')">
            <mat-icon>inbox</mat-icon>
            View Inbox Items
          </button>
        </div>
      </div>

      <!-- 4. DYNAMIC FORMS BY FORM TYPES (WIDGETS, SEARCH, GRID, DETAIL) -->
      <div class="forms-container">
        <!-- FORM TYPE: WIDGETS (KPI Metric Cards) -->
        <section class="form-section widgets-section">
          <div class="section-title-bar">
            <mat-icon class="section-icon blue">widgets</mat-icon>
            <div>
              <h3>Dashboard Metric Widgets</h3>
              <p class="section-subtitle">Real-time enterprise asset performance KPIs</p>
            </div>
          </div>

          <div class="widgets-grid">
            @for (widgetForm of widgetForms(); track widgetForm.formId) {
              <div class="widget-card" [style.border-top-color]="widgetForm.widgetConfig?.accentColor || '#3b82f6'">
                <div class="widget-header">
                  <span class="widget-caption">{{ widgetForm.caption }}</span>
                  <div class="widget-icon-box" [style.background-color]="widgetForm.widgetConfig?.accentColor + '20'" [style.color]="widgetForm.widgetConfig?.accentColor">
                    <mat-icon>{{ getMatIcon(widgetForm.widgetConfig?.icon || 'analytics') }}</mat-icon>
                  </div>
                </div>
                <h4 class="widget-title">{{ widgetForm.title }}</h4>
                <div class="widget-body">
                  <span class="metric-value">{{ widgetForm.widgetConfig?.metricValue }}</span>
                  <span 
                    class="trend-pill" 
                    [class.up]="widgetForm.widgetConfig?.trendDirection === 'up'"
                    [class.down]="widgetForm.widgetConfig?.trendDirection === 'down'"
                  >
                    <mat-icon>{{ widgetForm.widgetConfig?.trendDirection === 'up' ? 'trending_up' : 'trending_down' }}</mat-icon>
                    {{ widgetForm.widgetConfig?.metricTrend }}
                  </span>
                </div>
              </div>
            }
          </div>
        </section>

        <!-- FORM TYPE: SEARCH (Asset Criteria Filter Form) -->
        @if (searchFormSchema()) {
          <section class="form-section search-section">
            <div class="section-title-bar">
              <mat-icon class="section-icon emerald">search</mat-icon>
              <div>
                <h3>{{ searchFormMetadata()?.title || 'Asset Search & Filter Form' }}</h3>
                <p class="section-subtitle">{{ searchFormMetadata()?.caption || 'Filter active inventory and audit records' }}</p>
              </div>
            </div>

            <div class="search-form-card">
              <gp-am-form 
                [schema]="searchFormSchema()!"
                (formSubmit)="onSearchSubmit($event)"
              ></gp-am-form>
            </div>
          </section>
        }

        <!-- FORM TYPE: GRID (Inbox Action Items Table Form) -->
        @if (gridFormMetadata()) {
          <section class="form-section grid-section">
            <div class="section-title-bar">
              <mat-icon class="section-icon purple">table_chart</mat-icon>
              <div>
                <h3>{{ gridFormMetadata()?.title || 'Inbox Action Items & Maintenance Queue' }}</h3>
                <p class="section-subtitle">{{ gridFormMetadata()?.caption || 'Priority tasks requiring action' }}</p>
              </div>
            </div>

            <div class="grid-table-card">
              <table class="inbox-grid-table">
                <thead>
                  <tr>
                    <th>Task ID</th>
                    <th>Asset Barcode Tag</th>
                    <th>Device Name</th>
                    <th>Action Type</th>
                    <th>Priority SLA</th>
                    <th>Due Date</th>
                    <th>Status</th>
                    <th>Action</th>
                  </tr>
                </thead>
                <tbody>
                  @for (row of gridRows(); track row['id']) {
                    <tr>
                      <td class="cell-task-id">{{ row['id'] }}</td>
                      <td class="cell-tag"><code>{{ row['assetTag'] }}</code></td>
                      <td class="cell-name">{{ row['name'] }}</td>
                      <td class="cell-type">{{ row['type'] }}</td>
                      <td>
                        <span class="priority-pill" [class]="row['priority']?.toString()?.toLowerCase()">
                          {{ row['priority'] }}
                        </span>
                      </td>
                      <td class="cell-date">{{ row['dueDate'] }}</td>
                      <td>
                        <span class="status-pill" [class]="row['status']?.toString()?.toLowerCase()?.replace(' ', '-')">
                          {{ row['status'] }}
                        </span>
                      </td>
                      <td>
                        <button class="table-action-btn" title="Inspect Record">
                          <mat-icon>visibility</mat-icon>
                        </button>
                      </td>
                    </tr>
                  }
                </tbody>
              </table>
            </div>
          </section>
        }

        <!-- FORM TYPE: DETAIL (Read-Only Inspection Detail Record Form) -->
        @if (detailFormSchema()) {
          <section class="form-section detail-section">
            <div class="section-title-bar">
              <mat-icon class="section-icon amber">assignment</mat-icon>
              <div>
                <h3>{{ detailFormMetadata()?.title || 'Asset Inspection Record' }}</h3>
                <p class="section-subtitle">Read-only form mode (Labels placed on top)</p>
              </div>
            </div>

            <div class="detail-form-card">
              <gp-am-form 
                [schema]="detailFormSchema()!"
              ></gp-am-form>
            </div>
          </section>
        }
      </div>
    </div>
  `,
  styles: [`
    .dashboard-wrapper {
      padding: 24px;
      max-width: 1320px;
      margin: 0 auto;
      color: var(--text-primary, #f8fafc);
    }

    /* 1. SITE NAVIGATION LINKS BAR */
    .site-nav-bar {
      margin-bottom: 24px;
    }
    .site-nav-links {
      display: flex;
      gap: 12px;
      overflow-x: auto;
      padding-bottom: 4px;
    }
    .site-nav-item {
      display: flex;
      align-items: center;
      gap: 8px;
      padding: 10px 18px;
      background: rgba(30, 41, 59, 0.6);
      border: 1px solid rgba(255, 255, 255, 0.08);
      border-radius: 12px;
      color: #94a3b8;
      text-decoration: none;
      font-size: 0.875rem;
      font-weight: 600;
      transition: all 0.2s ease;
      white-space: nowrap;
    }
    .site-nav-item:hover {
      background: rgba(51, 65, 85, 0.8);
      color: #ffffff;
      border-color: rgba(99, 102, 241, 0.4);
    }
    .site-nav-item.active {
      background: linear-gradient(135deg, rgba(79, 70, 229, 0.3) 0%, rgba(6, 182, 212, 0.3) 100%);
      border-color: #6366f1;
      color: #ffffff;
      box-shadow: 0 0 16px rgba(99, 102, 241, 0.25);
    }
    .nav-count-badge {
      padding: 2px 7px;
      background: #ef4444;
      color: #ffffff;
      border-radius: 10px;
      font-size: 0.725rem;
      font-weight: 700;
    }

    /* 3. INBOX BANNER */
    .inbox-banner-card {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 24px 28px;
      background: linear-gradient(135deg, rgba(30, 41, 59, 0.9) 0%, rgba(15, 23, 42, 0.95) 100%);
      border: 1px solid rgba(99, 102, 241, 0.3);
      border-radius: 16px;
      margin-bottom: 32px;
      backdrop-filter: blur(12px);
      box-shadow: 0 8px 32px rgba(0, 0, 0, 0.2);
    }
    .inbox-banner-content {
      display: flex;
      align-items: center;
      gap: 20px;
    }
    .inbox-icon-container {
      position: relative;
      width: 56px;
      height: 56px;
      border-radius: 16px;
      background: rgba(99, 102, 241, 0.2);
      border: 1px solid rgba(99, 102, 241, 0.4);
      display: flex;
      align-items: center;
      justify-content: center;
      color: #818cf8;
    }
    .inbox-icon-container mat-icon {
      font-size: 28px;
      width: 28px;
      height: 28px;
    }
    .inbox-pulsing-badge {
      position: absolute;
      top: -4px;
      right: -4px;
      width: 22px;
      height: 22px;
      border-radius: 50%;
      background: #ef4444;
      color: #ffffff;
      font-size: 0.725rem;
      font-weight: 800;
      display: flex;
      align-items: center;
      justify-content: center;
      box-shadow: 0 0 10px #ef4444;
    }
    .inbox-text-content h2 {
      margin: 0 0 4px 0;
      font-size: 1.4rem;
      font-weight: 800;
      color: #ffffff;
    }
    .inbox-text-content p {
      margin: 0;
      color: #94a3b8;
      font-size: 0.95rem;
    }
    .quick-audit-btn {
      background: linear-gradient(135deg, #4f46e5 0%, #06b6d4 100%) !important;
      color: #ffffff !important;
      border-radius: 10px !important;
      padding: 0 20px !important;
      font-weight: 600 !important;
    }

    /* 4. FORM SECTIONS */
    .forms-container {
      display: flex;
      flex-direction: column;
      gap: 36px;
    }
    .form-section {
      background: rgba(15, 23, 42, 0.6);
      border: 1px solid rgba(255, 255, 255, 0.06);
      border-radius: 16px;
      padding: 24px;
    }
    .section-title-bar {
      display: flex;
      align-items: center;
      gap: 14px;
      margin-bottom: 20px;
    }
    .section-icon {
      width: 42px;
      height: 42px;
      border-radius: 12px;
      display: flex;
      align-items: center;
      justify-content: center;
    }
    .section-icon.blue { background: rgba(59, 130, 246, 0.15); color: #3b82f6; }
    .section-icon.emerald { background: rgba(16, 185, 129, 0.15); color: #10b981; }
    .section-icon.purple { background: rgba(168, 85, 247, 0.15); color: #a855f7; }
    .section-icon.amber { background: rgba(245, 158, 11, 0.15); color: #f59e0b; }

    .section-title-bar h3 {
      margin: 0;
      font-size: 1.15rem;
      font-weight: 700;
      color: #ffffff;
    }
    .section-subtitle {
      margin: 2px 0 0 0;
      font-size: 0.825rem;
      color: #64748b;
    }

    /* WIDGETS GRID */
    .widgets-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
      gap: 20px;
    }
    .widget-card {
      background: rgba(30, 41, 59, 0.7);
      border: 1px solid rgba(255, 255, 255, 0.08);
      border-top-width: 4px;
      border-radius: 14px;
      padding: 20px;
      transition: transform 0.2s ease, box-shadow 0.2s ease;
    }
    .widget-card:hover {
      transform: translateY(-2px);
      box-shadow: 0 10px 24px rgba(0, 0, 0, 0.2);
    }
    .widget-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 8px;
    }
    .widget-caption {
      font-size: 0.75rem;
      font-weight: 700;
      text-transform: uppercase;
      letter-spacing: 0.05em;
      color: #64748b;
    }
    .widget-icon-box {
      width: 36px;
      height: 36px;
      border-radius: 10px;
      display: flex;
      align-items: center;
      justify-content: center;
    }
    .widget-title {
      margin: 0 0 12px 0;
      font-size: 1rem;
      font-weight: 700;
      color: #f1f5f9;
    }
    .widget-body {
      display: flex;
      justify-content: space-between;
      align-items: baseline;
    }
    .metric-value {
      font-size: 1.75rem;
      font-weight: 800;
      color: #ffffff;
    }
    .trend-pill {
      display: inline-flex;
      align-items: center;
      gap: 4px;
      font-size: 0.775rem;
      font-weight: 600;
      padding: 3px 8px;
      border-radius: 8px;
    }
    .trend-pill.up {
      background: rgba(16, 185, 129, 0.15);
      color: #34d399;
    }
    .trend-pill.down {
      background: rgba(239, 68, 68, 0.15);
      color: #f87171;
    }

    /* GRID TABLE */
    .grid-table-card {
      overflow-x: auto;
      border-radius: 12px;
      border: 1px solid rgba(255, 255, 255, 0.08);
    }
    .inbox-grid-table {
      width: 100%;
      border-collapse: collapse;
      background: rgba(30, 41, 59, 0.5);
      font-size: 0.9rem;
    }
    .inbox-grid-table th {
      background: rgba(15, 23, 42, 0.9);
      padding: 14px 16px;
      text-align: left;
      font-size: 0.775rem;
      text-transform: uppercase;
      letter-spacing: 0.05em;
      color: #94a3b8;
      border-bottom: 1px solid rgba(255, 255, 255, 0.08);
    }
    .inbox-grid-table td {
      padding: 14px 16px;
      border-bottom: 1px solid rgba(255, 255, 255, 0.05);
      color: #cbd5e1;
    }
    .cell-task-id { font-weight: 700; color: #38bdf8; }
    .cell-tag code {
      background: rgba(0, 0, 0, 0.3);
      padding: 3px 6px;
      border-radius: 4px;
      color: #a7f3d0;
      font-family: monospace;
    }
    .priority-pill {
      padding: 3px 10px;
      border-radius: 10px;
      font-size: 0.75rem;
      font-weight: 700;
      text-transform: uppercase;
    }
    .priority-pill.high, .priority-pill.critical {
      background: rgba(239, 68, 68, 0.2);
      color: #f87171;
      border: 1px solid rgba(239, 68, 68, 0.4);
    }
    .priority-pill.medium {
      background: rgba(245, 158, 11, 0.2);
      color: #fbbf24;
      border: 1px solid rgba(245, 158, 11, 0.4);
    }
    .priority-pill.low {
      background: rgba(16, 185, 129, 0.2);
      color: #34d399;
      border: 1px solid rgba(16, 185, 129, 0.4);
    }
    .status-pill {
      padding: 3px 10px;
      border-radius: 10px;
      font-size: 0.75rem;
      font-weight: 600;
      background: rgba(148, 163, 184, 0.15);
      color: #cbd5e1;
    }
    .status-pill.pending-approval { background: rgba(59, 130, 246, 0.2); color: #60a5fa; }
    .status-pill.completed { background: rgba(16, 185, 129, 0.2); color: #34d399; }

    .table-action-btn {
      background: transparent;
      border: none;
      color: #94a3b8;
      cursor: pointer;
      padding: 4px;
      border-radius: 6px;
    }
    .table-action-btn:hover {
      color: #38bdf8;
      background: rgba(56, 189, 248, 0.15);
    }
  `]
})
export class DashboardComponent implements OnInit {
  userService = inject(UserService);
  formMetadataService = inject(FormMetadataService);

  activeSiteTab = signal<string>('nav-dashboard');

  readonly bootstrapData = computed<UserBootstrapData | null>(() => this.formMetadataService.bootstrapData());
  readonly profileLinks = computed<ProfileNavLink[]>(() => this.bootstrapData()?.profileNavLinks ?? []);
  readonly siteLinks = computed<SiteNavLink[]>(() => this.bootstrapData()?.siteNavLinks ?? []);
  readonly inboxCount = computed<number>(() => this.bootstrapData()?.inboxCount ?? 0);
  readonly dashboardForms = computed<DashboardFormMetadata[]>(() => this.bootstrapData()?.dashboardForms ?? []);

  // Form type extractions
  readonly widgetForms = computed<DashboardFormMetadata[]>(() =>
    this.dashboardForms().filter(f => f.formType === 'widget')
  );

  readonly searchFormMetadata = computed<DashboardFormMetadata | undefined>(() =>
    this.dashboardForms().find(f => f.formType === 'search')
  );

  readonly gridFormMetadata = computed<DashboardFormMetadata | undefined>(() =>
    this.dashboardForms().find(f => f.formType === 'grid')
  );

  readonly detailFormMetadata = computed<DashboardFormMetadata | undefined>(() =>
    this.dashboardForms().find(f => f.formType === 'detail')
  );

  // Form Schemas converted for gp-am-form component
  readonly searchFormSchema = computed<FormSchema | null>(() => {
    const meta = this.searchFormMetadata();
    if (!meta) return null;
    return {
      id: meta.formId,
      caption: meta.caption,
      title: meta.title,
      description: meta.description,
      formInfo: meta.formInfo,
      isEditable: meta.isEditable,
      labelPosition: meta.labelPosition,
      fields: meta.fields,
      submitButtonText: meta.searchConfig?.submitButtonLabel || 'Filter Results',
      showResetButton: true
    };
  });

  readonly detailFormSchema = computed<FormSchema | null>(() => {
    const meta = this.detailFormMetadata();
    if (!meta) return null;
    return {
      id: meta.formId,
      caption: meta.caption,
      title: meta.title,
      description: meta.description,
      formInfo: meta.formInfo,
      isEditable: meta.isEditable, // false -> labels on top
      labelPosition: 'top',
      fields: meta.fields,
      submitButtonText: 'Acknowledge Report',
      showResetButton: false
    };
  });

  readonly gridRows = computed<Record<string, any>[]>(() => {
    return this.gridFormMetadata()?.gridConfig?.rows ?? [];
  });

  ngOnInit(): void {
    // Automatically trigger user-bootstrap API call on login / startup
    this.formMetadataService.getUserBootstrap().subscribe();
  }

  onSearchSubmit(values: Record<string, any>): void {
    console.log('Dashboard Search Criteria Submitted:', values);
  }

  getMatIcon(iconName?: string): string {
    if (!iconName) return 'help_outline';
    const iconMap: Record<string, string> = {
      'user': 'person',
      'shield': 'security',
      'sliders': 'tune',
      'help-circle': 'help',
      'log-out': 'logout',
      'home': 'dashboard',
      'box': 'inventory_2',
      'check-circle': 'verified',
      'file-text': 'description',
      'bar-chart': 'bar_chart',
      'alert-triangle': 'warning',
      'shield-check': 'verified_user',
      'analytics': 'analytics'
    };
    return iconMap[iconName] || iconName;
  }
}
