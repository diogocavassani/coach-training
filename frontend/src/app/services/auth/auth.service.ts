import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, tap } from 'rxjs';

import { LoginRequest, LoginResponse } from './auth.models';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly loginEndpoint = '/auth/login';
  private readonly tokenStorageKey = 'coachtraining.auth.token';
  private readonly expirationStorageKey = 'coachtraining.auth.expiration';

  constructor(private readonly httpClient: HttpClient) {}

  login(payload: LoginRequest): Observable<LoginResponse> {
    return this.httpClient
      .post<LoginResponse>(this.loginEndpoint, payload)
      .pipe(tap((response) => this.persistSession(response)));
  }

  logout(): void {
    localStorage.removeItem(this.tokenStorageKey);
    localStorage.removeItem(this.expirationStorageKey);
  }

  getToken(): string | null {
    return localStorage.getItem(this.tokenStorageKey);
  }

  getExpiration(): Date | null {
    const expiration = localStorage.getItem(this.expirationStorageKey);
    if (!expiration) {
      return null;
    }

    const parsed = new Date(expiration);
    if (Number.isNaN(parsed.getTime())) {
      return null;
    }

    return parsed;
  }

  isAuthenticated(): boolean {
    const token = this.getToken();
    const expiration = this.getExpiration();

    if (!token || !expiration) {
      return false;
    }

    return expiration.getTime() > Date.now();
  }

  private persistSession(response: LoginResponse): void {
    localStorage.setItem(this.tokenStorageKey, response.token);
    localStorage.setItem(this.expirationStorageKey, response.expiraEmUtc);
  }
}
