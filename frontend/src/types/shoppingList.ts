export interface ShoppingListItem {
  name: string
  quantity: string
  sourceMeals: string[]
}

export interface ShoppingListPreviewRequest {
  mealPlanItemIds: string[]
}