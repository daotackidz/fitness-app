import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component, OnInit, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatSelectModule } from '@angular/material/select';
import { MatTableModule } from '@angular/material/table';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse, PageMeta } from '../../core/models/api-response.model';

interface ReportedContent {
  id: string;
  reporterUserId: string;
  reportableType: string;
  reportableId: string;
  reason: string | null;
  status: string;
  createdAt: string;
}

@Component({
  selector: 'app-reported-contents',
  standalone: true,
  imports: [CommonModule, MatTableModule, MatPaginatorModule, MatButtonModule, MatSelectModule],
  templateUrl: './reported-contents.component.html'
})
export class ReportedContentsComponent implements OnInit {
  readonly rows = signal<ReportedContent[]>([]);
  readonly meta = signal<PageMeta | null>(null);
  readonly displayedColumns = ['reportableType', 'reportableId', 'reason', 'status', 'createdAt', 'actions'];

  constructor(private http: HttpClient) {}

  ngOnInit(): void {
    this.load(1, 20);
  }

  async load(page: number, limit: number): Promise<void> {
    const response = await firstValueFrom(
      this.http.get<ApiResponse<ReportedContent[]>>(`${environment.apiBaseUrl}/admin/reported-contents`, {
        params: { page, limit }
      })
    );
    this.rows.set(response.data ?? []);
    this.meta.set(response.meta);
  }

  onPage(event: PageEvent): void {
    this.load(event.pageIndex + 1, event.pageSize);
  }

  async updateStatus(row: ReportedContent, status: string): Promise<void> {
    await firstValueFrom(this.http.patch(`${environment.apiBaseUrl}/admin/reported-contents/${row.id}`, { status }));
    this.load(this.meta()?.page ?? 1, this.meta()?.limit ?? 20);
  }

  async removeContent(row: ReportedContent): Promise<void> {
    if (!confirm('Go noi dung vi pham nay?')) return;
    const path = row.reportableType.toLowerCase() === 'forumpost' ? 'forum-posts' : 'comments';
    await firstValueFrom(this.http.delete(`${environment.apiBaseUrl}/admin/${path}/${row.reportableId}`));
    await this.updateStatus(row, 'reviewed');
  }
}
