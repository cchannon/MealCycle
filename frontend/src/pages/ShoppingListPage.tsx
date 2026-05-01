import { useEffect, useMemo, useState } from 'react'

import { useMealPlan } from '@/api/useMealPlan'
import { useShoppingListPreview } from '@/api/useShoppingList'

export function ShoppingListPage() {
  const mealPlanQuery = useMealPlan()
  const [selectedMealIds, setSelectedMealIds] = useState<string[]>([])
  const previewQuery = useShoppingListPreview(selectedMealIds)
  const [excludedIngredientNames, setExcludedIngredientNames] = useState<string[]>([])

  useEffect(() => {
    if (!mealPlanQuery.data || mealPlanQuery.data.length === 0) {
      return
    }

    setSelectedMealIds((current) => {
      if (current.length > 0) {
        return current
      }

      return mealPlanQuery.data.map((meal) => meal.id)
    })
  }, [mealPlanQuery.data])

  useEffect(() => {
    if (!previewQuery.data) {
      return
    }

    const availableNames = new Set(previewQuery.data.map((item) => item.name))
    setExcludedIngredientNames((current) => current.filter((name) => availableNames.has(name)))
  }, [previewQuery.data])

  function toggleMealSelection(mealPlanItemId: string) {
    setSelectedMealIds((current) =>
      current.includes(mealPlanItemId)
        ? current.filter((item) => item !== mealPlanItemId)
        : [...current, mealPlanItemId],
    )
  }

  function toggleIngredientExclusion(ingredientName: string) {
    setExcludedIngredientNames((current) =>
      current.includes(ingredientName)
        ? current.filter((name) => name !== ingredientName)
        : [...current, ingredientName],
    )
  }

  const selectedIngredientNames = useMemo(() => {
    if (!previewQuery.data) {
      return []
    }

    return previewQuery.data
      .map((item) => item.name)
      .filter((name) => !excludedIngredientNames.includes(name))
      .sort((left, right) => left.localeCompare(right))
  }, [previewQuery.data, excludedIngredientNames])

  return (
    <section className="panel-grid">
      <header className="page-header">
        <h2 className="page-title">Shopping List</h2>
        <p className="page-subtitle">
          MVP behavior: choose meals, then include or exclude ingredients before generating your list.
        </p>
      </header>

      <div className="card row">
        <h3>Select meals</h3>
        {mealPlanQuery.isLoading && <p>Loading meal plan...</p>}
        {mealPlanQuery.isError && <p className="error">Could not load meal plan items.</p>}
        {mealPlanQuery.data?.map((meal) => (
          <label className="shopping-item" key={meal.id}>
            <input
              type="checkbox"
              checked={selectedMealIds.includes(meal.id)}
              onChange={() => toggleMealSelection(meal.id)}
            />
            <span>
              {meal.day}: {meal.label}
            </span>
          </label>
        ))}
      </div>

      <div className="card row">
        <h3>Ingredient selection</h3>
        {selectedMealIds.length === 0 && <p>Select at least one meal to build a shopping list.</p>}
        {previewQuery.isLoading && selectedMealIds.length > 0 && <p>Generating ingredient list...</p>}
        {previewQuery.isError && <p className="error">Could not generate shopping list preview.</p>}
        {previewQuery.data?.map((ingredient) => (
          <label className="shopping-item" key={ingredient.name}>
            <input
              type="checkbox"
              checked={!excludedIngredientNames.includes(ingredient.name)}
              onChange={() => toggleIngredientExclusion(ingredient.name)}
            />
            <span>
              {ingredient.name} ({ingredient.quantity})
              <small className="shopping-meta">From: {ingredient.sourceMeals.join(', ')}</small>
            </span>
          </label>
        ))}
      </div>

      <div className="card row">
        <h3>Generated list preview ({selectedIngredientNames.length})</h3>
        {selectedIngredientNames.map((ingredientName) => (
          <label className="shopping-item" key={ingredientName}>
            <input type="checkbox" />
            <span>{ingredientName}</span>
          </label>
        ))}
      </div>
    </section>
  )
}
