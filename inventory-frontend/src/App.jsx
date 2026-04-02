// src/App.jsx
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom'
import { ThemeProvider } from './context/ThemeContext'
import { isLoggedIn }   from './api/inventoryApi'
import Sidebar        from './components/Sidebar'
import Topbar         from './components/Topbar'
import Dashboard      from './pages/Dashboard'
import Products       from './pages/Products'
import StockLevels    from './pages/StockLevels'
import Warehouses     from './pages/Warehouses'
import Categories     from './pages/Categories'
import Suppliers      from './pages/Suppliers'
import PurchaseOrders from './pages/PurchaseOrders'
import Batches        from './pages/Batches'
import Transactions   from './pages/Transactions'
import Reports        from './pages/Reports'
import Login          from './pages/Login'

/** Redirect to /login if no JWT token is in localStorage */
function ProtectedRoute({ children }) {
  return isLoggedIn() ? children : <Navigate to="/login" replace />
}

/** Redirect to / if already logged in */
function PublicRoute({ children }) {
  return isLoggedIn() ? <Navigate to="/" replace /> : children
}

function AppLayout() {
  return (
    <div className="layout">
      <Sidebar />
      <div className="main-area">
        <Topbar />
        <div className="page-content">
          <Routes>
            <Route path="/"                element={<Dashboard />} />
            <Route path="/products"        element={<Products />} />
            <Route path="/inventory"       element={<StockLevels />} />
            <Route path="/warehouses"      element={<Warehouses />} />
            <Route path="/categories"      element={<Categories />} />
            <Route path="/suppliers"       element={<Suppliers />} />
            <Route path="/purchase-orders" element={<PurchaseOrders />} />
            <Route path="/batches"         element={<Batches />} />
            <Route path="/transactions"    element={<Transactions />} />
            <Route path="/reports"         element={<Reports />} />
          </Routes>
        </div>
      </div>
    </div>
  )
}

export default function App() {
  return (
    <ThemeProvider>
      <BrowserRouter>
        <Routes>
          {/* Login — full-screen, no sidebar/topbar, redirect if already logged in */}
          <Route path="/login" element={<PublicRoute><Login /></PublicRoute>} />
          {/* App shell — protected, redirect to /login if not authenticated */}
          <Route path="/*" element={<ProtectedRoute><AppLayout /></ProtectedRoute>} />
        </Routes>
      </BrowserRouter>
    </ThemeProvider>
  )
}
