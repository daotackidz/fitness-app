import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component, OnInit, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatTableModule } from '@angular/material/table';
import { ActivatedRoute } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse, PageMeta } from '../../core/models/api-response.model';
import { SimpleCrudFormDialogComponent } from './simple-crud-form-dialog.component';
import { CrudResourceConfig } from './simple-crud.model';

@Component({
  selector: 'app-simple-crud-page',
  standalone: true,
  imports: [CommonModule, MatTableModule, MatPaginatorModule, MatButtonModule, MatIconModule, MatDialogModule],
  templateUrl: './simple-crud-page.component.html'
})
export class SimpleCrudPageComponent implements OnInit {
  config!: CrudResourceConfig;
  readonly rows = signal<Record<string, any>[]>([]);
  readonly meta = signal<PageMeta | null>(null);
  readonly loading = signal(false);

  get displayedColumns(): string[] {
    return [...this.config.columns.map((c) => c.key), 'actions'];
  }

  constructor(private route: ActivatedRoute, private http: HttpClient, private dialog: MatDialog) {}

  ngOnInit(): void {
    this.config = this.route.snapshot.data['resourceConfig'];
    this.load(1, 20);
  }

  async load(page: number, limit: number): Promise<void> {
    this.loading.set(true);
    try {
      const response = await firstValueFrom(
        this.http.get<ApiResponse<Record<string, any>[]>>(`${environment.apiBaseUrl}/${this.config.resourcePath}`, {
          params: { page, limit }
        })
      );
      this.rows.set(response.data ?? []);
      this.meta.set(response.meta);
    } finally {
      this.loading.set(false);
    }
  }

  onPage(event: PageEvent): void {
    this.load(event.pageIndex + 1, event.pageSize);
  }

  async openCreate(): Promise<void> {
    const dialogRef = this.dialog.open(SimpleCrudFormDialogComponent, {
      width: '480px',
      data: { config: this.config, record: null }
    });
    const result = await firstValueFrom(dialogRef.afterClosed());
    if (!result) return;

    await firstValueFrom(this.http.post(`${environment.apiBaseUrl}/${this.config.resourcePath}`, result));
    this.load(this.meta()?.page ?? 1, this.meta()?.limit ?? 20);
  }

  async openEdit(row: Record<string, any>): Promise<void> {
    const dialogRef = this.dialog.open(SimpleCrudFormDialogComponent, {
      width: '480px',
      data: { config: this.config, record: row }
    });
    const result = await firstValueFrom(dialogRef.afterClosed());
    if (!result) return;

    await firstValueFrom(this.http.patch(`${environment.apiBaseUrl}/${this.config.resourcePath}/${row['id']}`, result));
    this.load(this.meta()?.page ?? 1, this.meta()?.limit ?? 20);
  }

  async delete(row: Record<string, any>): Promise<void> {
    if (!confirm('Xoa muc nay?')) return;

    await firstValueFrom(this.http.delete(`${environment.apiBaseUrl}/${this.config.resourcePath}/${row['id']}`));
    this.load(this.meta()?.page ?? 1, this.meta()?.limit ?? 20);
  }
}
