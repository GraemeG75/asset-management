import { Component, inject, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet, Router, RouterModule } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { UserService } from './core/services/user.service';
import { FormMetadataService } from './core/services/form-metadata.service';
import { UiBlockerComponent } from './core/components/ui-blocker/ui-blocker.component';
import { ProfileNavLink } from './core/models/user-bootstrap.model';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    CommonModule,
    RouterOutlet,
    RouterModule,
    MatButtonModule,
    MatIconModule,
    MatMenuModule,
    UiBlockerComponent
  ],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  userService = inject(UserService);
  formMetadataService = inject(FormMetadataService);
  private router = inject(Router);

  readonly profileNavLinks = computed<ProfileNavLink[]>(() => {
    return this.formMetadataService.bootstrapData()?.profileNavLinks ?? [
      { id: 'profile-settings', label: 'My Profile', icon: 'person', url: '/profile', order: 1, isActive: true },
      { id: 'profile-security', label: 'Security & Credentials', icon: 'security', url: '/profile/security', order: 2, IsActive: true } as any,
      { id: 'profile-preferences', label: 'Language & Preferences', icon: 'tune', url: '/profile/preferences', order: 3, IsActive: true } as any,
      { id: 'profile-help', label: 'Help & Documentation', icon: 'help', url: '/help', order: 4, IsActive: true } as any,
      { id: 'profile-logout', label: 'Log Out', icon: 'logout', url: '/logout', order: 5, IsActive: true } as any
    ];
  });

  onLogout(): void {
    this.userService.logout();
    this.router.navigate(['/login']);
  }

  getMatIcon(iconName?: string): string {
    if (!iconName) return 'person';
    const iconMap: Record<string, string> = {
      'user': 'person',
      'shield': 'security',
      'sliders': 'tune',
      'help-circle': 'help',
      'log-out': 'logout'
    };
    return iconMap[iconName] || iconName;
  }

  getRoleLabel(roleId?: number): string {
    const roleMap: Record<number, string> = {
      1: 'Administrator',
      2: 'Asset Manager',
      3: 'Compliance Officer',
      4: 'Standard User',
      5: 'Read Only'
    };
    return roleMap[roleId ?? 4] || 'Standard User';
  }

  getRoleClass(roleId?: number): string {
    const classMap: Record<number, string> = {
      1: 'role-admin',
      2: 'role-manager',
      3: 'role-compliance',
      4: 'role-user',
      5: 'role-readonly'
    };
    return classMap[roleId ?? 4] || 'role-user';
  }
}
