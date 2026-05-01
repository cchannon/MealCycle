import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'

import { apiRequest } from '@/api/http'
import type { CookModeSession } from '@/types/cookMode'

function cookModeKey(mealPlanItemId: string) {
  return ['cook-mode', mealPlanItemId] as const
}

export function useCookModeSession(mealPlanItemId: string | undefined) {
  return useQuery({
    queryKey: mealPlanItemId ? cookModeKey(mealPlanItemId) : ['cook-mode', 'none'],
    queryFn: () => apiRequest<CookModeSession>(`/api/cook-mode/${mealPlanItemId}`),
    enabled: Boolean(mealPlanItemId),
  })
}

export function useSetCookStepCompletion(mealPlanItemId: string | undefined) {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: ({ stepIndex, isCompleted }: { stepIndex: number; isCompleted: boolean }) => {
      if (!mealPlanItemId) {
        throw new Error('No meal selected for cook mode.')
      }

      return apiRequest<CookModeSession>(`/api/cook-mode/${mealPlanItemId}/steps/${stepIndex}`, {
        method: 'PUT',
        body: JSON.stringify({ isCompleted }),
      })
    },
    onSuccess: (updatedSession) => {
      if (!mealPlanItemId) {
        return
      }

      queryClient.setQueryData(cookModeKey(mealPlanItemId), updatedSession)
    },
  })
}