export interface CookModeStep {
  stepIndex: number
  instruction: string
  isCompleted: boolean
}

export interface CookModeSession {
  mealPlanItemId: string
  day: string
  mealLabel: string
  steps: CookModeStep[]
}