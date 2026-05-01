import { DndContext, type DragEndEvent, useDroppable } from '@dnd-kit/core'
import {
  SortableContext,
  arrayMove,
  useSortable,
  verticalListSortingStrategy,
} from '@dnd-kit/sortable'
import { CSS } from '@dnd-kit/utilities'
import { useEffect, useState } from 'react'

import { useMealPlan, useCreateMealPlanItem, useReorderMealPlan } from '@/api/useMealPlan'
import { useRecipes } from '@/api/useRecipes'
import type { MealPlanItem, ReorderMealPlanInput } from '@/types/mealPlan'

interface PlannedMeal {
  id: string
  label: string
}

type MealColumns = Record<string, PlannedMeal[]>

const dayOrder = ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday']

const defaultColumns = dayOrder.reduce<MealColumns>((accumulator, day) => {
  accumulator[day] = []
  return accumulator
}, {})

function buildColumns(mealPlanItems: MealPlanItem[]): MealColumns {
  const columns = dayOrder.reduce<MealColumns>((accumulator, day) => {
    accumulator[day] = []
    return accumulator
  }, {})

  for (const mealPlanItem of mealPlanItems) {
    if (!columns[mealPlanItem.day]) {
      columns[mealPlanItem.day] = []
    }

    columns[mealPlanItem.day].push({
      id: mealPlanItem.id,
      label: mealPlanItem.label,
    })
  }

  return columns
}

function toReorderPayload(columns: MealColumns): ReorderMealPlanInput {
  const orderedItems: ReorderMealPlanInput['items'] = []

  for (const day of Object.keys(columns)) {
    columns[day].forEach((item, index) => {
      orderedItems.push({
        id: item.id,
        day,
        sortOrder: index,
      })
    })
  }

  return { items: orderedItems }
}

function SortableMealCard({ meal }: { meal: PlannedMeal }) {
  const { attributes, listeners, setNodeRef, transform, transition } = useSortable({ id: meal.id })

  const style = {
    transform: CSS.Transform.toString(transform),
    transition,
  }

  return (
    <article ref={setNodeRef} style={style} className="drag-card" {...attributes} {...listeners}>
      {meal.label}
    </article>
  )
}

function MealColumn({ day, meals }: { day: string; meals: PlannedMeal[] }) {
  const { setNodeRef, isOver } = useDroppable({ id: `column:${day}` })

  return (
    <section className="meal-column" key={day} ref={setNodeRef}>
      <h3>{day}</h3>
      <SortableContext items={meals.map((meal) => meal.id)} strategy={verticalListSortingStrategy}>
        {meals.map((meal) => (
          <SortableMealCard key={meal.id} meal={meal} />
        ))}
      </SortableContext>
      {meals.length === 0 && <p className={`meal-dropzone${isOver ? ' is-over' : ''}`}>Drop meal here</p>}
    </section>
  )
}

export function MealPlannerPage() {
  const mealPlanQuery = useMealPlan()
  const recipesQuery = useRecipes()
  const createMealPlanItem = useCreateMealPlanItem()
  const reorderMealPlan = useReorderMealPlan()
  const [columns, setColumns] = useState<MealColumns>(defaultColumns)
  const [selectedDay, setSelectedDay] = useState(dayOrder[0])
  const [selectedRecipeId, setSelectedRecipeId] = useState<string>('')

  useEffect(() => {
    if (!mealPlanQuery.data) {
      return
    }

    setColumns(buildColumns(mealPlanQuery.data))
  }, [mealPlanQuery.data])

  useEffect(() => {
    if (!recipesQuery.data || recipesQuery.data.length === 0) {
      return
    }

    setSelectedRecipeId((current) => current || recipesQuery.data[0].id)
  }, [recipesQuery.data])

  function onCreateMeal(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault()

    if (!selectedRecipeId) {
      return
    }

    createMealPlanItem.mutate({
      day: selectedDay,
      recipeId: selectedRecipeId,
    })
  }

  function findColumnByMealId(mealId: string): string | undefined {
    return Object.keys(columns).find((columnName) =>
      columns[columnName].some((meal) => meal.id === mealId),
    )
  }

  function onDragEnd(event: DragEndEvent) {
    const activeId = String(event.active.id)
    const overId = event.over ? String(event.over.id) : null

    if (!overId || activeId === overId) {
      return
    }

    const sourceColumn = findColumnByMealId(activeId)
    const destinationColumn = overId.startsWith('column:')
      ? overId.replace('column:', '')
      : findColumnByMealId(overId)

    if (!sourceColumn || !destinationColumn) {
      return
    }

    const sourceMeals = columns[sourceColumn]
    const destinationMeals = columns[destinationColumn]

    const sourceIndex = sourceMeals.findIndex((meal) => meal.id === activeId)
    const destinationIndex = overId.startsWith('column:')
      ? destinationMeals.length
      : destinationMeals.findIndex((meal) => meal.id === overId)

    if (destinationIndex < 0) {
      return
    }

    if (sourceColumn === destinationColumn) {
      const updated = arrayMove(sourceMeals, sourceIndex, destinationIndex)
      const nextColumns = {
        ...columns,
        [sourceColumn]: updated,
      }

      const previousColumns = columns
      setColumns(nextColumns)
      reorderMealPlan.mutate(toReorderPayload(nextColumns), {
        onError: () => {
          setColumns(previousColumns)
        },
      })
      return
    }

    const movedMeal = sourceMeals[sourceIndex]
    const nextSource = sourceMeals.filter((meal) => meal.id !== activeId)
    const nextDestination = [...destinationMeals]
    nextDestination.splice(destinationIndex, 0, movedMeal)

    const nextColumns = {
      ...columns,
      [sourceColumn]: nextSource,
      [destinationColumn]: nextDestination,
    }

    const previousColumns = columns
    setColumns(nextColumns)
    reorderMealPlan.mutate(toReorderPayload(nextColumns), {
      onError: () => {
        setColumns(previousColumns)
      },
    })
  }

  if (mealPlanQuery.isLoading) {
    return (
      <section className="panel-grid">
        <header className="page-header">
          <h2 className="page-title">Meal Plan Board</h2>
          <p className="page-subtitle">Loading planned meals...</p>
        </header>
      </section>
    )
  }

  if (mealPlanQuery.isError) {
    return (
      <section className="panel-grid">
        <header className="page-header">
          <h2 className="page-title">Meal Plan Board</h2>
          <p className="error">Could not load meal plan items.</p>
        </header>
      </section>
    )
  }

  return (
    <section className="panel-grid">
      <header className="page-header">
        <h2 className="page-title">Meal Plan Board</h2>
        <p className="page-subtitle">
          Drag meals between days or reorder them within a day to match ingredient availability.
        </p>
      </header>

      <form className="card row" onSubmit={onCreateMeal}>
        <h3>Add meal to plan</h3>
        <div className="panel-grid">
          <label className="field">
            <span className="label">Day</span>
            <select
              className="select"
              value={selectedDay}
              onChange={(event) => setSelectedDay(event.target.value)}
              disabled={createMealPlanItem.isPending}
            >
              {dayOrder.map((day) => (
                <option key={day} value={day}>
                  {day}
                </option>
              ))}
            </select>
          </label>

          <label className="field">
            <span className="label">Recipe</span>
            <select
              className="select"
              value={selectedRecipeId}
              onChange={(event) => setSelectedRecipeId(event.target.value)}
              disabled={recipesQuery.isLoading || createMealPlanItem.isPending || !recipesQuery.data?.length}
            >
              {recipesQuery.data?.map((recipe) => (
                <option key={recipe.id} value={recipe.id}>
                  {recipe.title}
                </option>
              ))}
            </select>
          </label>

          <button
            type="submit"
            className="button"
            disabled={createMealPlanItem.isPending || recipesQuery.isLoading || !selectedRecipeId}
          >
            {createMealPlanItem.isPending ? 'Adding...' : 'Add to meal plan'}
          </button>
        </div>
        {recipesQuery.isError && <p className="error">Could not load recipes for planning.</p>}
        {createMealPlanItem.isError && <p className="error">Could not add meal to plan.</p>}
      </form>

      {reorderMealPlan.isPending && <p className="page-subtitle">Saving meal order...</p>}

      <DndContext onDragEnd={onDragEnd}>
        <div className="meal-columns">
          {dayOrder.map((day) => (
            <MealColumn key={day} day={day} meals={columns[day] ?? []} />
          ))}
        </div>
      </DndContext>
    </section>
  )
}
