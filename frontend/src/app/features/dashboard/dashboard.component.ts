import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component, OnInit, signal } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../../core/models/api-response.model';

interface TopContent {
  id: string;
  name: string;
  score: number;
}

interface DashboardSummary {
  totalUsers: number;
  newUsersToday: number;
  newUsersThisWeek: number;
  openTickets: number;
  pendingReports: number;
  topRoutines: TopContent[];
  topArticles: TopContent[];
}

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, MatCardModule],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent implements OnInit {
  readonly summary = signal<DashboardSummary | null>(null);

  constructor(private http: HttpClient) {}

  async ngOnInit(): Promise<void> {
    const response = await firstValueFrom(
      this.http.get<ApiResponse<DashboardSummary>>(`${environment.apiBaseUrl}/admin/dashboard/summary`)
    );
    this.summary.set(response.data);
  }
}
