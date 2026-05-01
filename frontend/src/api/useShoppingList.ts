import { useQuery } from '@tanstack/react-query'

import { apiRequest } from '@/api/http'
import type { ShoppingListItem } from '@/types/shoppingList'

export function useShoppingListPreview(mealPlanItemIds: string[]) {
  const stableIds = [...mealPlanItemIds].sort((left, right) => left.localeCompare(right))

  return useQuery({
    queryKey: ['shopping-list-preview', stableIds],
    queryFn: () =>
      apiRequest<ShoppingListItem[]>('/api/shopping-list/preview', {
        method: 'POST',
        body: JSON.stringify({ mealPlanItemIds: stableIds }),
      }),
    enabled: stableIds.length > 0,
  })
}