export interface CrudFieldConfig {
  key: string;
  label: string;
  type: 'text' | 'textarea' | 'number' | 'select' | 'date' | 'file';
  options?: { value: string; label: string }[];
  /** For type 'select': load options at runtime from this admin API path (e.g. 'admin/categories?type=Article'). */
  optionsEndpoint?: string;
  required?: boolean;
  uploadContainer?: string;
  mediaKind?: 'video' | 'image';
  previewUrlKey?: string;
}

export interface CrudColumnConfig {
  key: string;
  label: string;
}

export interface CrudResourceConfig {
  title: string;
  resourcePath: string;
  columns: CrudColumnConfig[];
  fields: CrudFieldConfig[];
}
