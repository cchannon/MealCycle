import { NavLink } from 'react-router-dom'
import type { PropsWithChildren } from 'react'

import { cn } from '@/lib/cn'

const navItems = [
  { to: '/recipes', label: 'Recipes' },
  { to: '/meal-plan', label: 'Meal Plan' },
  { to: '/shopping', label: 'Shopping' },
  { to: '/cook', label: 'Cook Mode' },
  { to: '/ai-tools', label: 'AI Tools' },
]

export function AppShell({ children }: PropsWithChildren) {
  return (
    <div className="app-shell">
      <header className="app-header">
        <h1 className="app-title">MealCycle</h1>
        <nav className="nav-grid" aria-label="Primary">
          {navItems.map((item) => (
            <NavLink
              key={item.to}
              to={item.to}
              className={({ isActive }) => cn('nav-link', isActive && 'active')}
            >
              {item.label}
            </NavLink>
          ))}
        </nav>
      </header>
      <main className="page-shell">{children}</main>
    </div>
  )
}
