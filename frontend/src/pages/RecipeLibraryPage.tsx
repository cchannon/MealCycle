import { zodResolver } from '@hookform/resolvers/zod'
import { useForm } from 'react-hook-form'
import { z } from 'zod'

import { useCreateRecipe, useRecipes } from '@/api/useRecipes'

const formSchema = z.object({
  title: z.string().min(2, 'Title must be at least 2 characters.'),
  ingredients: z.string().min(2, 'Add at least one ingredient.'),
  steps: z.string().min(2, 'Add at least one step.'),
})

type RecipeFormData = z.infer<typeof formSchema>

export function RecipeLibraryPage() {
  const recipesQuery = useRecipes()
  const createRecipe = useCreateRecipe()

  const form = useForm<RecipeFormData>({
    resolver: zodResolver(formSchema),
    defaultValues: {
      title: '',
      ingredients: '',
      steps: '',
    },
  })

  async function onSubmit(values: RecipeFormData) {
    const ingredients = values.ingredients
      .split('\n')
      .map((line) => line.trim())
      .filter(Boolean)
      .map((line) => {
        const [name, quantity = 'to taste'] = line.split(',')
        return { name: name.trim(), quantity: quantity.trim() }
      })

    const steps = values.steps
      .split('\n')
      .map((line) => line.trim())
      .filter(Boolean)

    await createRecipe.mutateAsync({
      title: values.title,
      ingredients,
      steps,
    })

    form.reset()
  }

  return (
    <section className="panel-grid">
      <header className="page-header">
        <h2 className="page-title">Recipe Library</h2>
        <p className="page-subtitle">
          Save recipes here first, then pull them into meal plans and shopping lists.
        </p>
      </header>

      <div className="card row">
        <h3>Add Recipe</h3>
        <form className="row" onSubmit={form.handleSubmit(onSubmit)}>
          <label className="field">
            <span className="label">Title</span>
            <input className="input" {...form.register('title')} />
          </label>
          {form.formState.errors.title && <p className="error">{form.formState.errors.title.message}</p>}

          <label className="field">
            <span className="label">Ingredients</span>
            <textarea
              className="textarea"
              placeholder="One per line, optionally name, quantity"
              {...form.register('ingredients')}
            />
          </label>
          {form.formState.errors.ingredients && (
            <p className="error">{form.formState.errors.ingredients.message}</p>
          )}

          <label className="field">
            <span className="label">Steps</span>
            <textarea className="textarea" placeholder="One step per line" {...form.register('steps')} />
          </label>
          {form.formState.errors.steps && <p className="error">{form.formState.errors.steps.message}</p>}

          <button className="button" type="submit" disabled={createRecipe.isPending}>
            {createRecipe.isPending ? 'Saving...' : 'Save recipe'}
          </button>
        </form>
      </div>

      <div className="card row">
        <h3>Saved Recipes</h3>
        {recipesQuery.isLoading && <p>Loading recipes...</p>}
        {recipesQuery.isError && <p className="error">Could not load recipes.</p>}
        {recipesQuery.data?.map((recipe) => (
          <article key={recipe.id} className="recipe-row">
            <p className="recipe-title">{recipe.title}</p>
            <p className="recipe-meta">
              {recipe.ingredients.length} ingredients • {recipe.steps.length} steps
            </p>
          </article>
        ))}
      </div>
    </section>
  )
}
