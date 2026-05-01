import { useQuery } from '@tanstack/react-query'

import { apiRequest } from '@/api/http'
import type { HealthStatus } from '@/types/health'

const HEALTH_KEY = ['health'] as const

export function useHealthStatus() {
  return useQuery({
    queryKey: HEALTH_KEY,
    queryFn: () => apiRequest<HealthStatus>('/health'),
    refetchInterval: 15000,
  })
}
