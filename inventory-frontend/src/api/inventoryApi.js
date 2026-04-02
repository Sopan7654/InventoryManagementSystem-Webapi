// src/api/inventoryApi.js
// Centralized API client for all backend calls.
// All endpoints proxy through Vite to http://localhost:5000

const BASE = '/api'

// ── Auth token helpers ────────────────────────────────────────────────────────
export const getToken    = ()          => localStorage.getItem('ims_token')
export const setToken    = (token)     => localStorage.setItem('ims_token', token)
export const removeToken = ()          => localStorage.removeItem('ims_token')
export const getUser     = ()          => {
  const raw = localStorage.getItem('ims_user')
  return raw ? JSON.parse(raw) : null
}
export const setUser     = (user)      => localStorage.setItem('ims_user', JSON.stringify(user))
export const removeUser  = ()          => localStorage.removeItem('ims_user')
export const isLoggedIn  = ()          => !!getToken()

export const logout = async () => {
  const token = getToken()
  if (token) {
    try {
      await fetch(`${BASE}/auth/logout`, { 
        method: 'POST', 
        headers: { 'Authorization': `Bearer ${token}` } 
      })
    } catch {
      // Ignore network errors on logout, just burn the local session
    }
  }
  removeToken()
  removeUser()
  window.location.href = '/login'
}

async function fetchJSON(path, options = {}) {
  const token = getToken()
  const headers = {
    'Content-Type': 'application/json',
    ...(token ? { 'Authorization': `Bearer ${token}` } : {}),
    ...options.headers,
  }
  const res = await fetch(`${BASE}${path}`, { headers, ...options })
  if (!res.ok) {
    const err = await res.json().catch(() => ({ message: res.statusText }))
    
    let errorMsg = err?.detail?.message || err?.message || err?.error;
    
    // Check for ASP.NET Core FluentValidation ProblemDetails pattern
    if (!errorMsg && err?.errors && typeof err.errors === 'object') {
      const firstKey = Object.keys(err.errors)[0];
      if (firstKey && Array.isArray(err.errors[firstKey])) {
        errorMsg = err.errors[firstKey][0];
      }
    }
    
    throw new Error(errorMsg || `HTTP ${res.status}: Validation or Server Error`)
  }
  const json = await res.json()
  
  // Unpack CQRS Result<T> from backend format into Axios-style `{ data }` format
  if (json && typeof json.isSuccess === 'boolean') {
    if (!json.isSuccess) throw new Error(json.error || 'Server error occurred')
    return { data: json.value }
  }
  return json
}

// ── Auth ──────────────────────────────────────────────────────────────────────
export const loginUser = async (username, password) => {
  const response = await fetchJSON('/auth/login', {
    method: 'POST',
    body: JSON.stringify({ username, password }),
  })
  
  if (!response?.data) throw new Error("Invalid response from server");

  const user = response.data;
  setToken(user.token)
  setUser({ userId: user.userId, username: user.username, email: user.email, role: user.role })
  return user
}

export const registerUser = async (username, email, password) => {
  const response = await fetchJSON('/auth/register', {
    method: 'POST',
    body: JSON.stringify({ username, email, password }),
  })
  
  if (!response?.data) throw new Error("Invalid response from server");

  const user = response.data;
  setToken(user.token)
  setUser({ userId: user.userId, username: user.username, email: user.email, role: user.role })
  return user
}


// ── Products ──────────────────────────────────────────────────────────────────
export const getProducts      = () => fetchJSON('/products')
export const getProductById   = (id) => fetchJSON(`/products/${id}`)
export const createProduct    = (data) => fetchJSON('/products', { method: 'POST', body: JSON.stringify(data) })
export const updateProduct    = (id, data) => fetchJSON(`/products/${id}`, { method: 'PUT', body: JSON.stringify(data) })
export const deleteProduct    = (id) => fetchJSON(`/products/${id}`, { method: 'DELETE' })
export const importProducts   = (products) => fetchJSON('/products/import', { method: 'POST', body: JSON.stringify({ products }) })

// ── Inventory ─────────────────────────────────────────────────────────────────
export const getStockLevels   = () => fetchJSON('/inventory/stock-levels')
export const stockIn          = (data) => fetchJSON('/inventory/stock-in', { method: 'POST', body: JSON.stringify(data) })
export const stockOut         = (data) => fetchJSON('/inventory/stock-out', { method: 'POST', body: JSON.stringify(data) })
export const transfer         = (data) => fetchJSON('/inventory/transfer', { method: 'POST', body: JSON.stringify(data) })
export const holdStock        = (data) => fetchJSON('/inventory/hold', { method: 'POST', body: JSON.stringify(data) })
export const adjustment       = (data) => fetchJSON('/inventory/adjustment', { method: 'POST', body: JSON.stringify(data) })
export const getValuation     = (productId, method = 'FIFO', warehouseId) => {
  const params = new URLSearchParams({ productId, method })
  if (warehouseId) params.set('warehouseId', warehouseId)
  return fetchJSON(`/inventory/valuation?${params}`)
}

// ── Warehouses ────────────────────────────────────────────────────────────────
export const getWarehouses    = () => fetchJSON('/warehouses')
export const createWarehouse  = (data) => fetchJSON('/warehouses', { method: 'POST', body: JSON.stringify(data) })
export const updateWarehouse  = (id, data) => fetchJSON(`/warehouses/${id}`, { method: 'PUT', body: JSON.stringify(data) })

// ── Categories ────────────────────────────────────────────────────────────────
export const getCategories    = () => fetchJSON('/categories')
export const createCategory   = (data) => fetchJSON('/categories', { method: 'POST', body: JSON.stringify(data) })
export const updateCategory   = (id, data) => fetchJSON(`/categories/${id}`, { method: 'PUT', body: JSON.stringify(data) })

// ── Suppliers ─────────────────────────────────────────────────────────────────
export const getSuppliers     = () => fetchJSON('/suppliers')
export const createSupplier   = (data) => fetchJSON('/suppliers', { method: 'POST', body: JSON.stringify(data) })

// ── Purchase Orders ───────────────────────────────────────────────────────────
export const getPurchaseOrders = () => fetchJSON('/purchase-orders')
export const createPurchaseOrder = (data) => fetchJSON('/purchase-orders', { method: 'POST', body: JSON.stringify(data) })
export const receivePurchaseOrder = (id, warehouseId) => fetchJSON(`/purchase-orders/${id}/status`, { method: 'PATCH', body: JSON.stringify({ purchaseOrderId: id, status: 'RECEIVED', warehouseId }) })
export const getPurchaseOrderItems = (id) => fetchJSON(`/purchase-orders/${id}/items`)

// ── Batches ───────────────────────────────────────────────────────────────────
export const getBatches       = () => fetchJSON('/batches')
export const createBatch      = (data) => fetchJSON('/batches', { method: 'POST', body: JSON.stringify(data) })

// ── Reports ───────────────────────────────────────────────────────────────────
export const getLowStock      = () => fetchJSON('/reports/low-stock')
export const getExpiringBatches = (days = 30) => fetchJSON(`/reports/expiring-batches?days=${days}`)
export const getTransactions  = (productId, limit = 50) => {
  const params = new URLSearchParams({ limit })
  if (productId) params.set('productId', productId)
  return fetchJSON(`/reports/transactions?${params}`)
}
export const getOverstock     = (threshold = 3) => fetchJSON(`/reports/overstock?threshold=${threshold}`)
export const getAbcAnalysis   = () => fetchJSON('/reports/abc-analysis')
export const getStockAging    = () => fetchJSON('/reports/stock-aging')
export const getTurnover      = (from, to) => fetchJSON(`/reports/turnover?from=${from}&to=${to}`)
