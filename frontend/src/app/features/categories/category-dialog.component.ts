import { CommonModule } from '@angular/common';
import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { Select } from 'primeng/select';
import { CATEGORY_TYPE_OPTIONS, Category } from './category.model';

export interface CategoryDialogData {
  record: Category | null;
  defaultType: string;
}

@Component({
  selector: 'app-category-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    Select
  ],
  templateUrl: './category-dialog.component.html'
})
export class CategoryDialogComponent {
  readonly typeOptions = CATEGORY_TYPE_OPTIONS;
  readonly isEdit: boolean;
  readonly form: FormGroup;

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<CategoryDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: CategoryDialogData
  ) {
    this.isEdit = !!data.record;
    this.form = this.fb.group({
      name: [data.record?.name ?? '', [Validators.required, Validators.maxLength(100)]],
      type: [{ value: data.record?.type ?? data.defaultType, disabled: this.isEdit }, [Validators.required]]
    });
  }

  save(): void {
    if (this.form.invalid) return;
    this.dialogRef.close(this.form.getRawValue());
  }

  cancel(): void {
    this.dialogRef.close(null);
  }
}
