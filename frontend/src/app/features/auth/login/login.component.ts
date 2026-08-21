import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { UserService } from '../../../core/services/user.service';
import { TranslationService, LanguageOption } from '../../../core/services/translation.service';
import { SsoProviderId } from '../../../core/models/user.model';
import { TranslatePipe } from '../../../core/pipes/translate.pipe';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    TranslatePipe
  ],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent implements OnInit {
  private fb = inject(FormBuilder);
  private userService = inject(UserService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  translationService = inject(TranslationService);

  loginForm: FormGroup = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(4)]],
    rememberMe: [true]
  });

  isLoading = signal(false);
  activeSsoProvider = signal<SsoProviderId | null>(null);
  errorMessage = signal<string | null>(null);
  hidePassword = signal(true);

  get availableLanguages(): LanguageOption[] {
    return this.translationService.availableLanguages;
  }

  get currentCulture(): string {
    return this.translationService.currentCulture();
  }

  ngOnInit(): void {
    if (this.userService.isLoggedIn()) {
      const returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';
      this.router.navigateByUrl(returnUrl);
    }
  }

  onLanguageChange(event: Event): void {
    const select = event.target as HTMLSelectElement;
    if (select && select.value) {
      this.translationService.setCulture(select.value);
    }
  }

  onSubmit(): void {
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }

    this.isLoading.set(true);
    this.errorMessage.set(null);

    const { email, password, rememberMe } = this.loginForm.value;

    this.userService.login({ email, password, rememberMe }).subscribe({
      next: () => {
        this.isLoading.set(false);
        this.navigateAfterLogin();
      },
      error: (err) => {
        this.isLoading.set(false);
        this.errorMessage.set(err.message || 'Authentication failed. Please check your credentials.');
      }
    });
  }

  onSsoLogin(provider: SsoProviderId): void {
    this.isLoading.set(true);
    this.activeSsoProvider.set(provider);
    this.errorMessage.set(null);

    const rememberMe = this.loginForm.get('rememberMe')?.value ?? true;

    this.userService.loginWithSso(provider, rememberMe).subscribe({
      next: () => {
        this.isLoading.set(false);
        this.activeSsoProvider.set(null);
        this.navigateAfterLogin();
      },
      error: (err) => {
        this.isLoading.set(false);
        this.activeSsoProvider.set(null);
        this.errorMessage.set(err.message || `SSO login with ${provider} failed. Please try again.`);
      }
    });
  }

  private navigateAfterLogin(): void {
    const returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';
    const target = (returnUrl === '/login') ? '/' : returnUrl;
    this.router.navigateByUrl(target);
  }

  togglePasswordVisibility(): void {
    this.hidePassword.update(val => !val);
  }
}
