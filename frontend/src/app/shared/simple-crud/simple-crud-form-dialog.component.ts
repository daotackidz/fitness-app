import { CommonModule } from '@angular/common';
import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
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
    MatSelectModule,
    MatButtonModule
  ],
  templateUrl: './simple-crud-form-dialog.component.html'
})
export class SimpleCrudFormDialogComponent {
  readonly form: FormGroup;

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<SimpleCrudFormDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: SimpleCrudDialogData
  ) {
    const group: Record<string, any> = {};
    for (const field of data.config.fields) {
      const initialValue = data.record?.[field.key] ?? '';
      group[field.key] = [initialValue, field.required ? [Validators.required] : []];
    }
    this.form = this.fb.group(group);
  }

  save(): void {
    if (this.form.invalid) return;
    this.dialogRef.close(this.form.getRawValue());
  }

  cancel(): void {
    this.dialogRef.close(null);
  }
}
