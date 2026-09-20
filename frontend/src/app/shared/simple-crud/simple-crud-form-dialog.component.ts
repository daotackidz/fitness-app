import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component, Inject, signal } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { Select } from 'primeng/select';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../../core/models/api-response.model';
import { CrudResourceConfig } from './simple-crud.model';

export interface SimpleCrudDialogData {
  config: CrudResourceConfig;
  record: Record<string, any> | null;
}

@Component({
  selector: 'app-simple-crud-form-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    Select
  ],
  templateUrl: './simple-crud-form-dialog.component.html'
})
export class SimpleCrudFormDialogComponent {
  readonly form: FormGroup;
  readonly uploadingFields = signal<Record<string, boolean>>({});
  readonly previewUrls = signal<Record<string, string | null>>({});
  readonly dynamicOptions = signal<Record<string, { value: string; label: string }[]>>({});

  constructor(
    private fb: FormBuilder,
    private http: HttpClient,
    private dialogRef: MatDialogRef<SimpleCrudFormDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: SimpleCrudDialogData
  ) {
    const group: Record<string, any> = {};
    const initialPreviews: Record<string, string | null> = {};
    for (const field of data.config.fields) {
      const initialValue = data.record?.[field.key] ?? '';
      group[field.key] = [initialValue, field.required ? [Validators.required] : []];
      if (field.type === 'file' && field.previewUrlKey) {
        initialPreviews[field.key] = data.record?.[field.previewUrlKey] ?? null;
      }
      if (field.type === 'select' && field.optionsEndpoint) {
        this.loadOptions(field.key, field.optionsEndpoint);
      }
    }
    this.form = this.fb.group(group);
    this.previewUrls.set(initialPreviews);
  }

  optionsFor(field: { options?: { value: string; label: string }[]; optionsEndpoint?: string }, key: string): { value: string; label: string }[] {
    if (field.options) return field.options;
    return this.dynamicOptions()[key] ?? [];
  }

  private async loadOptions(key: string, endpoint: string): Promise<void> {
    const response = await firstValueFrom(
      this.http.get<ApiResponse<{ name: string }[]>>(`${environment.apiBaseUrl}/${endpoint}`)
    );
    const options = (response.data ?? []).map((item) => ({ value: item.name, label: item.name }));
    this.dynamicOptions.update((state) => ({ ...state, [key]: options }));
  }

  isUploading(key: string): boolean {
    return !!this.uploadingFields()[key];
  }

  previewUrl(key: string): string | null {
    return this.previewUrls()[key] ?? null;
  }

  async onFileSelected(event: Event, key: string, container: string, mediaKind: string): Promise<void> {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    if (!file) return;

    this.uploadingFields.update((state) => ({ ...state, [key]: true }));
    try {
      const formData = new FormData();
      formData.append('file', file);
      const response = await firstValueFrom(
        this.http.post<ApiResponse<{ id: string; url: string }>>(
          `${environment.apiBaseUrl}/admin/uploads?container=${container}&type=${mediaKind}`,
          formData
        )
      );
      this.form.get(key)?.setValue(response.data!.id);
      this.previewUrls.update((state) => ({ ...state, [key]: response.data!.url }));
    } finally {
      this.uploadingFields.update((state) => ({ ...state, [key]: false }));
    }
  }

  save(): void {
    if (this.form.invalid) return;
    this.dialogRef.close(this.form.getRawValue());
  }

  cancel(): void {
    this.dialogRef.close(null);
  }
}
