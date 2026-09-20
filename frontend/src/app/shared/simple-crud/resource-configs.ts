import { CrudResourceConfig } from './simple-crud.model';

const DIFFICULTY_OPTIONS = [
  { value: 'beginner', label: 'Beginner' },
  { value: 'intermediate', label: 'Intermediate' },
  { value: 'advanced', label: 'Advanced' }
];

export const EXERCISES_CONFIG: CrudResourceConfig = {
  title: 'Bài tập',
  resourcePath: 'admin/exercises',
  columns: [
    { key: 'name', label: 'Tên' },
    { key: 'muscleGroup', label: 'Nhóm cơ' },
    { key: 'difficultyLevel', label: 'Cấp độ' }
  ],
  fields: [
    { key: 'name', label: 'Tên bài tập', type: 'text', required: true },
    { key: 'description', label: 'Mô tả', type: 'textarea' },
    { key: 'muscleGroup', label: 'Nhóm cơ', type: 'text' },
    { key: 'equipment', label: 'Dụng cụ', type: 'text' },
    { key: 'difficultyLevel', label: 'Cấp độ', type: 'select', options: DIFFICULTY_OPTIONS, required: true },
    { key: 'videoFileId', label: 'Video hướng dẫn', type: 'file', uploadContainer: 'exercises', mediaKind: 'video', previewUrlKey: 'videoUrl', required: true },
    { key: 'imageFileId', label: 'Ảnh minh họa', type: 'file', uploadContainer: 'exercises', mediaKind: 'image', previewUrlKey: 'imageUrl', required: true },
    { key: 'caloriesEstimate', label: 'Calo ước tính', type: 'number' }
  ]
};

export const ROUTINES_CONFIG: CrudResourceConfig = {
  title: 'Routine',
  resourcePath: 'admin/routines',
  columns: [
    { key: 'name', label: 'Tên' },
    { key: 'level', label: 'Cấp độ' },
    { key: 'durationWeeks', label: 'Số tuần' }
  ],
  fields: [
    { key: 'name', label: 'Tên routine', type: 'text', required: true },
    { key: 'level', label: 'Cấp độ', type: 'select', options: DIFFICULTY_OPTIONS, required: true },
    { key: 'description', label: 'Mô tả', type: 'textarea' },
    { key: 'durationWeeks', label: 'Số tuần', type: 'number' }
  ]
};

export const MEAL_PLANS_CONFIG: CrudResourceConfig = {
  title: 'Meal Plan',
  resourcePath: 'admin/meal-plans',
  columns: [
    { key: 'name', label: 'Tên' },
    { key: 'goal', label: 'Mục tiêu' },
    { key: 'totalCalories', label: 'Tổng calo' }
  ],
  fields: [
    { key: 'name', label: 'Tên meal plan', type: 'text', required: true },
    { key: 'goal', label: 'Mục tiêu', type: 'text' },
    { key: 'description', label: 'Mô tả', type: 'textarea' },
    { key: 'totalCalories', label: 'Tổng calo', type: 'number' }
  ]
};

export const ARTICLES_CONFIG: CrudResourceConfig = {
  title: 'Article',
  resourcePath: 'admin/articles',
  columns: [
    { key: 'title', label: 'Tiêu đề' },
    { key: 'category', label: 'Chuyên mục' },
    { key: 'author', label: 'Tác giả' }
  ],
  fields: [
    { key: 'title', label: 'Tiêu đề', type: 'text', required: true },
    { key: 'content', label: 'Nội dung', type: 'textarea' },
    { key: 'category', label: 'Chuyên mục', type: 'select', optionsEndpoint: 'admin/categories?type=Article&limit=100' },
    { key: 'coverImageId', label: 'Ảnh bìa', type: 'file', uploadContainer: 'articles', mediaKind: 'image', previewUrlKey: 'coverImage' },
    { key: 'author', label: 'Tác giả', type: 'text' }
  ]
};

export const VIDEOS_CONFIG: CrudResourceConfig = {
  title: 'Video',
  resourcePath: 'admin/videos',
  columns: [
    { key: 'title', label: 'Tiêu đề' },
    { key: 'category', label: 'Chuyên mục' },
    { key: 'durationSeconds', label: 'Thời lượng (s)' }
  ],
  fields: [
    { key: 'title', label: 'Tiêu đề', type: 'text', required: true },
    { key: 'description', label: 'Mô tả', type: 'textarea' },
    { key: 'videoFileId', label: 'File video', type: 'file', uploadContainer: 'videos', mediaKind: 'video', previewUrlKey: 'videoUrl', required: true },
    { key: 'thumbnailImageId', label: 'Thumbnail', type: 'file', uploadContainer: 'videos', mediaKind: 'image', previewUrlKey: 'thumbnailUrl' },
    { key: 'durationSeconds', label: 'Thời lượng (giây)', type: 'number' },
    { key: 'category', label: 'Chuyên mục', type: 'select', optionsEndpoint: 'admin/categories?type=Video&limit=100' }
  ]
};

export const FAQS_CONFIG: CrudResourceConfig = {
  title: 'FAQ',
  resourcePath: 'admin/faqs',
  columns: [
    { key: 'question', label: 'Câu hỏi' },
    { key: 'category', label: 'Chuyên mục' }
  ],
  fields: [
    { key: 'question', label: 'Câu hỏi', type: 'text', required: true },
    { key: 'answer', label: 'Câu trả lời', type: 'textarea', required: true },
    { key: 'category', label: 'Chuyên mục', type: 'select', optionsEndpoint: 'admin/categories?type=Faq&limit=100' }
  ]
};

export const CHALLENGES_CONFIG: CrudResourceConfig = {
  title: 'Challenge',
  resourcePath: 'admin/challenges',
  columns: [
    { key: 'name', label: 'Tên' },
    { key: 'type', label: 'Loại' },
    { key: 'startDate', label: 'Bắt đầu' },
    { key: 'endDate', label: 'Kết thúc' }
  ],
  fields: [
    { key: 'name', label: 'Tên thử thách', type: 'text', required: true },
    { key: 'description', label: 'Mô tả', type: 'textarea' },
    {
      key: 'type',
      label: 'Loại',
      type: 'select',
      required: true,
      options: [
        { value: 'weekly', label: 'Weekly' },
        { value: 'competition', label: 'Competition' }
      ]
    },
    { key: 'startDate', label: 'Ngày bắt đầu', type: 'date', required: true },
    { key: 'endDate', label: 'Ngày kết thúc', type: 'date', required: true },
    { key: 'goalMetric', label: 'Chỉ tiêu', type: 'text' },
    { key: 'reward', label: 'Phần thưởng', type: 'text' }
  ]
};
