export interface RecipeIngredient {
  name: string
  quantity: string
}

export interface Recipe {
  id: string
  title: string
  ingredients: RecipeIngredient[]
  steps: string[]
  updatedAtUtc: string
}

export interface CreateRecipeInput {
  title: string
  ingredients: RecipeIngredient[]
  steps: string[]
}
