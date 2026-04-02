// src/components/Sidebar.jsx
import { NavLink } from 'react-router-dom'
import {
  LayoutDashboard, Package, Layers, Building2, Tag,
  Handshake, ClipboardList, FlaskConical, ArrowLeftRight,
  BarChart3, Boxes, Zap,
} from 'lucide-react'
import { cn } from '../lib/utils'

const sections = [
  {
    label: 'Overview',
    items: [{ to: '/', icon: LayoutDashboard, label: 'Dashboard', exact: true }],
  },
  {
    label: 'Catalog',
    items: [
      { to: '/products',   icon: Package,   label: 'Products' },
      { to: '/categories', icon: Tag,        label: 'Categories' },
      { to: '/suppliers',  icon: Handshake,  label: 'Suppliers' },
    ],
  },
  {
    label: 'Inventory',
    items: [
      { to: '/inventory',    icon: Layers,          label: 'Stock Levels' },
      { to: '/warehouses',   icon: Building2,        label: 'Warehouses' },
      { to: '/batches',      icon: FlaskConical,     label: 'Batches' },
      { to: '/transactions', icon: ArrowLeftRight,   label: 'Transactions' },
    ],
  },
  {
    label: 'Procurement',
    items: [{ to: '/purchase-orders', icon: ClipboardList, label: 'Purchase Orders' }],
  },
  {
    label: 'Analytics',
    items: [{ to: '/reports', icon: BarChart3, label: 'Reports' }],
  },
]

export default function Sidebar() {
  return (
    <aside className="sidebar">
      {/* Brand */}
      <div className="sidebar-brand">
        <div className="brand-icon">
          <Boxes size={18} color="#fff" />
        </div>
        <div>
          <p className="brand-name">IMS Pro</p>
          <p className="brand-tagline">Inventory System</p>
        </div>
      </div>

      {/* Nav sections */}
      <nav className="sidebar-scroll">
        {sections.map((sec) => (
          <div key={sec.label}>
            <p className="nav-section-label">{sec.label}</p>
            {sec.items.map(({ to, icon: Icon, label, exact }) => (
              <NavLink
                key={to}
                to={to}
                end={exact}
                className={({ isActive }) => cn('nav-item', isActive && 'active')}
              >
                <span className="nav-icon-wrap">
                  <Icon size={15} strokeWidth={2} />
                </span>
                {label}
              </NavLink>
            ))}
          </div>
        ))}
      </nav>
    </aside>
  )
}
