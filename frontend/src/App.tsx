import { Navigate, Route, Routes } from 'react-router-dom'

import { AppShell } from '@/components/AppShell'
import { AiToolsPage } from '@/pages/AiToolsPage'
import { CookModePage } from '@/pages/CookModePage'
import { MealPlannerPage } from '@/pages/MealPlannerPage'
import { RecipeLibraryPage } from '@/pages/RecipeLibraryPage'
import { ShoppingListPage } from '@/pages/ShoppingListPage'

function App() {
  return (
    <AppShell>
      <Routes>
        <Route path="/recipes" element={<RecipeLibraryPage />} />
        <Route path="/meal-plan" element={<MealPlannerPage />} />
        <Route path="/shopping" element={<ShoppingListPage />} />
        <Route path="/cook" element={<CookModePage />} />
        <Route path="/ai-tools" element={<AiToolsPage />} />
        <Route path="*" element={<Navigate to="/recipes" replace />} />
      </Routes>
    </AppShell>
  )
}

export default App
