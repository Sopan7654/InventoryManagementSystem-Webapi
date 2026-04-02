// src/components/Topbar.jsx
import { useLocation } from 'react-router-dom'
import { useTheme } from '../context/ThemeContext'
import { Moon, Sun, LogOut, User } from 'lucide-react'
import { cn } from '../lib/utils'
import { getUser, logout } from '../api/inventoryApi'

const pages = {
  '/':                'Dashboard',
  '/products':        'Products',
  '/categories':      'Categories',
  '/suppliers':       'Suppliers',
  '/inventory':       'Stock Levels',
  '/warehouses':      'Warehouses',
  '/batches':         'Batches',
  '/transactions':    'Transactions',
  '/purchase-orders': 'Purchase Orders',
  '/reports':         'Reports',
}

const themes = [
  { value: 'dark',  Icon: Moon, title: 'Dark' },
  { value: 'light', Icon: Sun,  title: 'Light' },
]

export default function Topbar() {
  const loc = useLocation()
  const { theme, setTheme } = useTheme()
  const page = pages[loc.pathname] ?? 'IMS Pro'
  const user = getUser()

  return (
    <header className="topbar">
      <div className="topbar-left">
        <div className="breadcrumb">
          <span style={{ color: 'hsl(var(--muted-foreground))' }}>Inventory Management System</span>
          <span className="breadcrumb-sep">/</span>
          <span className="breadcrumb-current">{page}</span>
        </div>
      </div>

      <div className="topbar-right">
        {/* User profile & Logout */}
        {user && (
          <div style={{ display: 'flex', alignItems: 'center', gap: '1rem', marginRight: '0.75rem', paddingRight: '0.75rem', borderRight: '1px solid hsl(var(--border))' }}>
            <div style={{ display: 'flex', alignItems: 'center', gap: '0.5rem' }}>
              <div style={{ width: '28px', height: '28px', borderRadius: '50%', backgroundColor: 'hsl(var(--primary))', color: 'hsl(var(--primary-foreground))', display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
                <User size={15} />
              </div>
              <div style={{ display: 'flex', flexDirection: 'column' }}>
                <span style={{ fontSize: '0.8125rem', fontWeight: 500, lineHeight: 1.1 }}>{user.username}</span>
                <span style={{ fontSize: '0.65rem', color: 'hsl(var(--muted-foreground))' }}>{user.role}</span>
              </div>
            </div>
            
            <button 
              onClick={logout}
              title="Logout"
              style={{ display: 'flex', alignItems: 'center', justifyContent: 'center', background: 'transparent', border: '1px solid transparent', color: 'hsl(var(--muted-foreground))', cursor: 'pointer', padding: '0.35rem', borderRadius: 'var(--radius)', transition: 'all 0.2s' }}
              onMouseOver={e => { e.currentTarget.style.color = 'hsl(var(--destructive))'; e.currentTarget.style.background = 'hsl(var(--destructive) / 0.1)' }}
              onMouseOut={e => { e.currentTarget.style.color = 'hsl(var(--muted-foreground))'; e.currentTarget.style.background = 'transparent' }}
            >
              <LogOut size={16} />
            </button>
          </div>
        )}

        {/* Theme toggle */}
        <div className="theme-group" role="group" aria-label="Theme selection">
          {themes.map(({ value, Icon, title }) => (
            <button
              key={value}
              className={cn('theme-btn', theme === value && 'active')}
              onClick={() => setTheme(value)}
              title={title}
              aria-label={title}
            >
              <Icon size={13} strokeWidth={2} />
            </button>
          ))}
        </div>
      </div>
    </header>
  )
}
