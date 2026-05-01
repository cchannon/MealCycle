export interface InfrastructureReadiness {
  persistenceProvider: string
  usingInMemoryPersistence: boolean
  tableStorageConfigured: boolean
  foundryConfigured: boolean
}

export interface HealthStatus {
  status: string
  infrastructureReadiness: InfrastructureReadiness
}
