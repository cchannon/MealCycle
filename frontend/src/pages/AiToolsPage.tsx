import { useHealthStatus } from '@/api/useHealth'

const aiCapabilities = [
  'Recipe suggestions from household meal history',
  'Ingredient substitutions with rationale',
  'Import recipe from URL into editable draft',
  'Import handwritten recipe photo into editable draft',
]

export function AiToolsPage() {
  const healthQuery = useHealthStatus()

  const readiness = healthQuery.data?.infrastructureReadiness

  return (
    <section className="panel-grid">
      <header className="page-header">
        <h2 className="page-title">AI Tools</h2>
        <p className="page-subtitle">
          Foundry-backed capabilities will be orchestrated by the backend, with draft confirmation before save.
        </p>
      </header>

      <div className="card row">
        {aiCapabilities.map((capability) => (
          <article key={capability} className="recipe-row">
            <p className="recipe-title">{capability}</p>
            <p className="recipe-meta">Planned for iterative rollout after core meal planning baseline.</p>
          </article>
        ))}
      </div>

      <div className="card row">
        <h3>Infrastructure readiness</h3>
        {healthQuery.isLoading && <p className="page-subtitle">Checking backend readiness...</p>}
        {healthQuery.isError && <p className="error">Could not retrieve readiness status.</p>}
        {readiness && (
          <>
            <p className="recipe-meta">Persistence provider: {readiness.persistenceProvider}</p>
            <p className="recipe-meta">
              Table Storage configured: {readiness.tableStorageConfigured ? 'Yes' : 'No'}
            </p>
            <p className="recipe-meta">
              Foundry configured: {readiness.foundryConfigured ? 'Yes' : 'No'}
            </p>
            <p className="recipe-meta">
              In-memory persistence active: {readiness.usingInMemoryPersistence ? 'Yes' : 'No'}
            </p>
          </>
        )}
      </div>
    </section>
  )
}
