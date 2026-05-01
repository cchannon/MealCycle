export interface MealPlanItem {
  id: string
  recipeId: string
  day: string
  label: string
  sortOrder: number
  updatedAtUtc: string
}

export interface MealPlanItemOrderInput {
  id: string
  day: string
  sortOrder: number
}

export interface ReorderMealPlanInput {
  items: MealPlanItemOrderInput[]
}

export interface CreateMealPlanItemInput {
  day: string
  recipeId: string
}