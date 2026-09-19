import { CrudResourceConfig } from './simple-crud.model';

const DIFFICULTY_OPTIONS = [
  { value: 'beginner', label: 'Beginner' },
  { value: 'intermediate', label: 'Intermediate' },
  { value: 'advanced', label: 'Advanced' }
];

export const EXERCISES_CONFIG: CrudResourceConfig = {
  title: 'Bai tap',
  resourcePath: 'admin/exercises',
  columns: [
    { key: 'name', label: 'Ten' },
    { key: 'muscleGroup', label: 'Nhom co' },
    { key: 'difficultyLevel', label: 'Cap do' }
  ],
  fields: [
    { key: 'name', label: 'Ten bai tap', type: 'text', required: true },
    { key: 'description', label: 'Mo ta', type: 'textarea' },
    { key: 'muscleGroup', label: 'Nhom co', type: 'text' },
    { key: 'equipment', label: 'Dung cu', type: 'text' },
    { key: 'difficultyLevel', label: 'Cap do', type: 'select', options: DIFFICULTY_OPTIONS, required: true },
    { key: 'videoFileId', label: 'Video huong dan', type: 'file', uploadContainer: 'exercises', mediaKind: 'video', previewUrlKey: 'videoUrl' },
    { key: 'imageFileId', label: 'Anh minh hoa', type: 'file', uploadContainer: 'exercises', mediaKind: 'image', previewUrlKey: 'imageUrl' },
    { key: 'caloriesEstimate', label: 'Calo uoc tinh', type: 'number' }
  ]
};

export const ROUTINES_CONFIG: CrudResourceConfig = {
  title: 'Routine',
  resourcePath: 'admin/routines',
  columns: [
    { key: 'name', label: 'Ten' },
    { key: 'level', label: 'Cap do' },
    { key: 'durationWeeks', label: 'So tuan' }
  ],
  fields: [
    { key: 'name', label: 'Ten routine', type: 'text', required: true },
    { key: 'level', label: 'Cap do', type: 'select', options: DIFFICULTY_OPTIONS, required: true },
    { key: 'description', label: 'Mo ta', type: 'textarea' },
    { key: 'durationWeeks', label: 'So tuan', type: 'number' }
  ]
};

export const MEAL_PLANS_CONFIG: CrudResourceConfig = {
  title: 'Meal Plan',
  resourcePath: 'admin/meal-plans',
  columns: [
    { key: 'name', label: 'Ten' },
    { key: 'goal', label: 'Muc tieu' },
    { key: 'totalCalories', label: 'Tong calo' }
  ],
  fields: [
    { key: 'name', label: 'Ten meal plan', type: 'text', required: true },
    { key: 'goal', label: 'Muc tieu', type: 'text' },
    { key: 'description', label: 'Mo ta', type: 'textarea' },
    { key: 'totalCalories', label: 'Tong calo', type: 'number' }
  ]
};

export const ARTICLES_CONFIG: CrudResourceConfig = {
  title: 'Article',
  resourcePath: 'admin/articles',
  columns: [
    { key: 'title', label: 'Tieu de' },
    { key: 'category', label: 'Chuyen muc' },
    { key: 'author', label: 'Tac gia' }
  ],
  fields: [
    { key: 'title', label: 'Tieu de', type: 'text', required: true },
    { key: 'content', label: 'Noi dung', type: 'textarea' },
    { key: 'category', label: 'Chuyen muc', type: 'text' },
    { key: 'coverImageId', label: 'Anh bia', type: 'file', uploadContainer: 'articles', mediaKind: 'image', previewUrlKey: 'coverImage' },
    { key: 'author', label: 'Tac gia', type: 'text' }
  ]
};

export const VIDEOS_CONFIG: CrudResourceConfig = {
  title: 'Video',
  resourcePath: 'admin/videos',
  columns: [
    { key: 'title', label: 'Tieu de' },
    { key: 'category', label: 'Chuyen muc' },
    { key: 'durationSeconds', label: 'Thoi luong (s)' }
  ],
  fields: [
    { key: 'title', label: 'Tieu de', type: 'text', required: true },
    { key: 'description', label: 'Mo ta', type: 'textarea' },
    { key: 'videoFileId', label: 'File video', type: 'file', uploadContainer: 'videos', mediaKind: 'video', previewUrlKey: 'videoUrl', required: true },
    { key: 'thumbnailImageId', label: 'Thumbnail', type: 'file', uploadContainer: 'videos', mediaKind: 'image', previewUrlKey: 'thumbnailUrl' },
    { key: 'durationSeconds', label: 'Thoi luong (giay)', type: 'number' },
    { key: 'category', label: 'Chuyen muc', type: 'text' }
  ]
};

export const FAQS_CONFIG: CrudResourceConfig = {
  title: 'FAQ',
  resourcePath: 'admin/faqs',
  columns: [
    { key: 'question', label: 'Cau hoi' },
    { key: 'category', label: 'Chuyen muc' }
  ],
  fields: [
    { key: 'question', label: 'Cau hoi', type: 'text', required: true },
    { key: 'answer', label: 'Cau tra loi', type: 'textarea', required: true },
    { key: 'category', label: 'Chuyen muc', type: 'text' }
  ]
};

export const CHALLENGES_CONFIG: CrudResourceConfig = {
  title: 'Challenge',
  resourcePath: 'admin/challenges',
  columns: [
    { key: 'name', label: 'Ten' },
    { key: 'type', label: 'Loai' },
    { key: 'startDate', label: 'Bat dau' },
    { key: 'endDate', label: 'Ket thuc' }
  ],
  fields: [
    { key: 'name', label: 'Ten thu thach', type: 'text', required: true },
    { key: 'description', label: 'Mo ta', type: 'textarea' },
    {
      key: 'type',
      label: 'Loai',
      type: 'select',
      required: true,
      options: [
        { value: 'weekly', label: 'Weekly' },
        { value: 'competition', label: 'Competition' }
      ]
    },
    { key: 'startDate', label: 'Ngay bat dau', type: 'date', required: true },
    { key: 'endDate', label: 'Ngay ket thuc', type: 'date', required: true },
    { key: 'goalMetric', label: 'Chi tieu', type: 'text' },
    { key: 'reward', label: 'Phan thuong', type: 'text' }
  ]
};
