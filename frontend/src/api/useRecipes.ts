import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'

import { apiRequest } from '@/api/http'
import type { CreateRecipeInput, Recipe } from '@/types/recipes'

const RECIPES_KEY = ['recipes'] as const

export function useRecipes() {
  return useQuery({
    queryKey: RECIPES_KEY,
    queryFn: () => apiRequest<Recipe[]>('/api/recipes'),
  })
}

export function useCreateRecipe() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (request: CreateRecipeInput) =>
      apiRequest<Recipe>('/api/recipes', {
        method: 'POST',
        body: JSON.stringify(request),
      }),
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: RECIPES_KEY })
    },
  })
}
