import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component, OnInit, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatTableModule } from '@angular/material/table';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../../environments/environment';
import { TranslatePipe } from '../../core/i18n/translate.pipe';
import { ApiResponse, PageMeta } from '../../core/models/api-response.model';
import { CategoryDialogComponent } from './category-dialog.component';
import { CATEGORY_TYPE_OPTIONS, Category } from './category.model';

@Component({
  selector: 'app-categories',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatPaginatorModule,
    MatButtonModule,
    MatButtonToggleModule,
    MatIconModule,
    MatDialogModule,
    TranslatePipe
  ],
  templateUrl: './categories.component.html'
})
export class CategoriesComponent implements OnInit {
  readonly typeOptions = CATEGORY_TYPE_OPTIONS;
  readonly activeType = signal<string>(CATEGORY_TYPE_OPTIONS[0].value);
  readonly rows = signal<Category[]>([]);
  readonly meta = signal<PageMeta | null>(null);
  readonly loading = signal(false);
  readonly displayedColumns = ['name', 'usageCount', 'createdAt', 'actions'];

  constructor(private http: HttpClient, private dialog: MatDialog) {}

  ngOnInit(): void {
    this.load(1, 20);
  }

  selectType(type: string): void {
    this.activeType.set(type);
    this.load(1, this.meta()?.limit ?? 20);
  }

  async load(page: number, limit: number): Promise<void> {
    this.loading.set(true);
    try {
      const response = await firstValueFrom(
        this.http.get<ApiResponse<Category[]>>(`${environment.apiBaseUrl}/admin/categories`, {
          params: { page, limit, type: this.activeType() }
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
    const dialogRef = this.dialog.open(CategoryDialogComponent, {
      width: '420px',
      data: { record: null, defaultType: this.activeType() }
    });
    const result = await firstValueFrom(dialogRef.afterClosed());
    if (!result) return;

    await firstValueFrom(this.http.post(`${environment.apiBaseUrl}/admin/categories`, result));
    this.load(this.meta()?.page ?? 1, this.meta()?.limit ?? 20);
  }

  async openEdit(row: Category): Promise<void> {
    const dialogRef = this.dialog.open(CategoryDialogComponent, {
      width: '420px',
      data: { record: row, defaultType: row.type }
    });
    const result = await firstValueFrom(dialogRef.afterClosed());
    if (!result) return;

    await firstValueFrom(this.http.patch(`${environment.apiBaseUrl}/admin/categories/${row.id}`, result));
    this.load(this.meta()?.page ?? 1, this.meta()?.limit ?? 20);
  }

  async delete(row: Category): Promise<void> {
    if (!confirm(`Xóa category "${row.name}"?`)) return;

    await firstValueFrom(this.http.delete(`${environment.apiBaseUrl}/admin/categories/${row.id}`));
    this.load(this.meta()?.page ?? 1, this.meta()?.limit ?? 20);
  }
}
