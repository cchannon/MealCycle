import { useEffect, useMemo, useState } from 'react'

import { useCookModeSession, useSetCookStepCompletion } from '@/api/useCookMode'
import { useMealPlan } from '@/api/useMealPlan'

export function CookModePage() {
  const mealPlanQuery = useMealPlan()
  const [selectedMealPlanItemId, setSelectedMealPlanItemId] = useState<string | undefined>(undefined)

  useEffect(() => {
    if (!mealPlanQuery.data || mealPlanQuery.data.length === 0) {
      return
    }

    setSelectedMealPlanItemId((current) => current ?? mealPlanQuery.data[0].id)
  }, [mealPlanQuery.data])

  const cookSessionQuery = useCookModeSession(selectedMealPlanItemId)
  const setStepCompletion = useSetCookStepCompletion(selectedMealPlanItemId)

  const completedCount = useMemo(() => {
    if (!cookSessionQuery.data) {
      return 0
    }

    return cookSessionQuery.data.steps.filter((step) => step.isCompleted).length
  }, [cookSessionQuery.data])

  function onToggleStep(stepIndex: number, isCompleted: boolean) {
    setStepCompletion.mutate({ stepIndex, isCompleted: !isCompleted })
  }

  return (
    <section className="panel-grid">
      <header className="page-header">
        <h2 className="page-title">Cook Mode</h2>
        <p className="page-subtitle">
          Kitchen-first step progression with large controls and persisted completion state.
        </p>
      </header>

      <div className="card row">
        <label className="field">
          <span className="label">Meal to cook</span>
          <select
            className="select"
            value={selectedMealPlanItemId ?? ''}
            onChange={(event) => setSelectedMealPlanItemId(event.target.value)}
            disabled={mealPlanQuery.isLoading || !mealPlanQuery.data?.length}
          >
            {mealPlanQuery.data?.map((item) => (
              <option key={item.id} value={item.id}>
                {item.day}: {item.label}
              </option>
            ))}
          </select>
        </label>
        {mealPlanQuery.isError && <p className="error">Could not load meal plan entries.</p>}
      </div>

      <div className="card row">
        {cookSessionQuery.isLoading && <p>Loading cook steps...</p>}
        {cookSessionQuery.isError && <p className="error">Could not load cook session.</p>}
        {!cookSessionQuery.isLoading && cookSessionQuery.data && (
          <p className="page-subtitle">
            {cookSessionQuery.data.mealLabel} • {completedCount}/{cookSessionQuery.data.steps.length} steps complete
          </p>
        )}
        {!cookSessionQuery.isLoading && cookSessionQuery.data?.steps.length === 0 && (
          <p>No recipe steps were found for this planned meal yet.</p>
        )}
        {cookSessionQuery.data?.steps.map((step) => (
          <article key={step.stepIndex} className="recipe-row">
            <label className="shopping-item">
              <input
                type="checkbox"
                checked={step.isCompleted}
                onChange={() => onToggleStep(step.stepIndex, step.isCompleted)}
                disabled={setStepCompletion.isPending}
              />
              <span>
                <span className="recipe-title">Step {step.stepIndex + 1}</span>
                <span className="shopping-meta">{step.instruction}</span>
              </span>
            </label>
          </article>
        ))}
      </div>
    </section>
  )
}
