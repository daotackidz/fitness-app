import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatSelectModule } from '@angular/material/select';
import { MatTableModule } from '@angular/material/table';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse, PageMeta } from '../../core/models/api-response.model';
import { AuthService } from '../../core/services/auth.service';

interface AdminUser {
  id: string;
  fullName: string;
  email: string;
  phone: string | null;
  role: string;
  status: string;
  createdAt: string;
}

@Component({
  selector: 'app-users',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatTableModule,
    MatPaginatorModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule
  ],
  templateUrl: './users.component.html',
  styleUrl: './users.component.scss'
})
export class UsersComponent implements OnInit {
  readonly rows = signal<AdminUser[]>([]);
  readonly meta = signal<PageMeta | null>(null);
  readonly displayedColumns = ['fullName', 'email', 'role', 'status', 'createdAt', 'actions'];
  searchTerm = '';

  constructor(private http: HttpClient, public authService: AuthService) {}

  ngOnInit(): void {
    this.load(1, 20);
  }

  async load(page: number, limit: number): Promise<void> {
    const response = await firstValueFrom(
      this.http.get<ApiResponse<AdminUser[]>>(`${environment.apiBaseUrl}/admin/users`, {
        params: { page, limit, q: this.searchTerm }
      })
    );
    this.rows.set(response.data ?? []);
    this.meta.set(response.meta);
  }

  onPage(event: PageEvent): void {
    this.load(event.pageIndex + 1, event.pageSize);
  }

  search(): void {
    this.load(1, this.meta()?.limit ?? 20);
  }

  async toggleLock(user: AdminUser): Promise<void> {
    const newStatus = user.status.toLowerCase() === 'locked' ? 'active' : 'locked';
    await firstValueFrom(
      this.http.patch(`${environment.apiBaseUrl}/admin/users/${user.id}/status`, { status: newStatus })
    );
    this.load(this.meta()?.page ?? 1, this.meta()?.limit ?? 20);
  }

  async changeRole(user: AdminUser, role: string): Promise<void> {
    await firstValueFrom(this.http.patch(`${environment.apiBaseUrl}/admin/users/${user.id}/role`, { role }));
    this.load(this.meta()?.page ?? 1, this.meta()?.limit ?? 20);
  }
}
