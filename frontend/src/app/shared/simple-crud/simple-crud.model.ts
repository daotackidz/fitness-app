export interface CrudFieldConfig {
  key: string;
  label: string;
  type: 'text' | 'textarea' | 'number' | 'select' | 'date';
  options?: { value: string; label: string }[];
  required?: boolean;
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
