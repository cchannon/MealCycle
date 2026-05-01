import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'

import { apiRequest } from '@/api/http'
import type { CreateMealPlanItemInput, MealPlanItem, ReorderMealPlanInput } from '@/types/mealPlan'

const MEAL_PLAN_KEY = ['meal-plan'] as const

export function useMealPlan() {
  return useQuery({
    queryKey: MEAL_PLAN_KEY,
    queryFn: () => apiRequest<MealPlanItem[]>('/api/meal-plan'),
  })
}

export function useReorderMealPlan() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (request: ReorderMealPlanInput) =>
      apiRequest<MealPlanItem[]>('/api/meal-plan/reorder', {
        method: 'PUT',
        body: JSON.stringify(request),
      }),
    onSuccess: (updatedItems) => {
      queryClient.setQueryData(MEAL_PLAN_KEY, updatedItems)
    },
  })
}

export function useCreateMealPlanItem() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (request: CreateMealPlanItemInput) =>
      apiRequest<MealPlanItem>('/api/meal-plan', {
        method: 'POST',
        body: JSON.stringify(request),
      }),
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: MEAL_PLAN_KEY })
    },
  })
}