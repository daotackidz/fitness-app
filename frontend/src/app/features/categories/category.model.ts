export interface Category {
  id: string;
  name: string;
  type: string;
  usageCount: number;
  createdAt: string;
}

export const CATEGORY_TYPE_OPTIONS = [
  { value: 'Article', label: 'Article' },
  { value: 'Video', label: 'Video' },
  { value: 'Faq', label: 'FAQ' },
  { value: 'Food', label: 'Food' }
];
