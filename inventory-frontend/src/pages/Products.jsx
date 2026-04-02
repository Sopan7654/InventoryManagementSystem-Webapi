// src/pages/Products.jsx
import { useState, useEffect } from 'react'
import { Plus, Search, Pencil, PowerOff } from 'lucide-react'
import { getProducts, createProduct, updateProduct, deleteProduct, getCategories } from '../api/inventoryApi'
import PageHeader from '../components/PageHeader'
import Modal from '../components/Modal'
import Toast from '../components/Toast'
import LoadingSpinner from '../components/LoadingSpinner'

const EMPTY = { sku: '', productName: '', description: '', categoryId: '', unitOfMeasure: 'PCS', cost: '', listPrice: '', isActive: true }

function FormField({ label, children }) {
  return (
    <div>
      <label className="label">{label}</label>
      {children}
    </div>
  )
}

export default function Products() {
  const [products, setProducts]     = useState([])
  const [categories, setCategories] = useState([])
  const [loading, setLoading]       = useState(true)
  const [search, setSearch]         = useState('')
  const [modal, setModal]           = useState(null)
  const [form, setForm]             = useState(EMPTY)
  const [editId, setEditId]         = useState(null)
  const [toast, setToast]           = useState(null)
  const [saving, setSaving]         = useState(false)

  const reload = () =>
    Promise.all([getProducts(), getCategories()])
      .then(([p, c]) => { setProducts(p.data || []); setCategories(c.data || []) })
      .finally(() => setLoading(false))

  useEffect(() => { reload() }, [])

  const openCreate = () => { setForm(EMPTY); setEditId(null); setModal('create') }
  const openEdit = (p) => {
    setForm({ sku: p.sku, productName: p.productName, description: p.description || '', categoryId: p.categoryId || '', unitOfMeasure: p.unitOfMeasure || 'PCS', cost: p.cost, listPrice: p.listPrice, isActive: p.isActive })
    setEditId(p.productId)
    setModal('edit')
  }

  const handleSave = async () => {
    setSaving(true)
    try {
      if (editId) await updateProduct(editId, form)
      else        await createProduct(form)
      setToast({ message: editId ? 'Product updated!' : 'Product created!', type: 'success' })
      setModal(null); reload()
    } catch (e) { setToast({ message: e.message, type: 'error' }) }
    finally { setSaving(false) }
  }

  const handleDelete = async (id, name) => {
    if (!window.confirm(`Deactivate "${name}"?`)) return
    try {
      await deleteProduct(id)
      setToast({ message: `${name} deactivated.`, type: 'success' }); reload()
    } catch (e) { setToast({ message: e.message, type: 'error' }) }
  }

  const handleReactivate = async (p) => {
    if (!window.confirm(`Reactivate "${p.productName}"?`)) return
    setSaving(true)
    try {
      await updateProduct(p.productId, { ...p, isActive: true })
      setToast({ message: `${p.productName} reactivated.`, type: 'success' }); reload()
    } catch (e) { setToast({ message: e.message, type: 'error' }) }
    finally { setSaving(false) }
  }

  const filtered = products.filter(p =>
    [p.productName, p.sku].some(f => f?.toLowerCase().includes(search.toLowerCase()))
  )

  if (loading) return <LoadingSpinner />

  return (
    <div className="anim-fade-in">
      <PageHeader
        title="Products"
        subtitle={`${products.filter(p => p.isActive).length} active · ${products.length} total products`}
        actions={
          <button className="btn btn-default" onClick={openCreate}>
            <Plus size={15} strokeWidth={2.5} /> Add Product
          </button>
        }
      />

      {/* Search bar */}
      <div style={{ marginBottom: '1rem' }}>
        <div className="search-input-wrap" style={{ maxWidth: 300 }}>
          <Search size={15} className="search-icon-inner" />
          <input
            className="input"
            placeholder="Search by name or SKU…"
            value={search}
            onChange={e => setSearch(e.target.value)}
          />
        </div>
      </div>

      {/* Data table */}
      <div className="card" style={{ overflow: 'hidden' }}>
        <div className="data-table-wrap">
          <table className="data-table">
            <thead>
              <tr>
                <th>SKU</th>
                <th>Product Name</th>
                <th>Category</th>
                <th>Unit</th>
                <th style={{ textAlign: 'right' }}>Cost</th>
                <th style={{ textAlign: 'right' }}>List Price</th>
                <th>Status</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              {filtered.length === 0 ? (
                <tr>
                  <td colSpan={8}>
                    <div className="empty-state">
                      <span className="empty-icon">📦</span>
                      <p className="empty-title">No products found</p>
                      <p className="empty-body">Try a different search or add a new product</p>
                    </div>
                  </td>
                </tr>
              ) : filtered.map(p => (
                <tr key={p.productId}>
                  <td>
                    <span className="mono" style={{ fontSize: '0.8125rem', color: 'hsl(var(--primary))', fontWeight: 600 }}>
                      {p.sku}
                    </span>
                  </td>
                  <td style={{ fontWeight: 600, color: 'hsl(var(--foreground))' }}>{p.productName}</td>
                  <td>{p.categoryName || <span style={{ opacity: 0.4 }}>—</span>}</td>
                  <td><span className="badge badge-secondary">{p.unitOfMeasure}</span></td>
                  <td style={{ textAlign: 'right', fontFamily: "'JetBrains Mono', monospace", fontSize: '0.8125rem' }}>
                    ₹{Number(p.cost).toLocaleString()}
                  </td>
                  <td style={{ textAlign: 'right', fontFamily: "'JetBrains Mono', monospace", fontSize: '0.8125rem' }}>
                    ₹{Number(p.listPrice).toLocaleString()}
                  </td>
                  <td>
                    {p.isActive
                      ? <span className="badge badge-success">● Active</span>
                      : <span className="badge badge-secondary">○ Inactive</span>
                    }
                  </td>
                  <td>
                    <div style={{ display: 'flex', gap: '0.375rem', alignItems: 'center' }}>
                      <button className="btn btn-outline btn-sm" onClick={() => openEdit(p)}>
                        <Pencil size={12} /> Edit
                      </button>
                      {p.isActive ? (
                        <button className="btn btn-destructive btn-sm" onClick={() => handleDelete(p.productId, p.productName)}>
                          <PowerOff size={12} /> Deactivate
                        </button>
                      ) : (
                        <button className="btn btn-outline btn-sm" onClick={() => handleReactivate(p)} style={{ color: 'hsl(var(--success))', borderColor: 'hsl(var(--success)/0.3)' }}>
                          <PowerOff size={12} /> Reactivate
                        </button>
                      )}
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>

      {modal && (
        <Modal title={modal === 'edit' ? 'Edit Product' : 'New Product'} onClose={() => setModal(null)}>
          <div style={{ display: 'flex', flexDirection: 'column', gap: '1rem' }}>
            {[
              ['SKU', 'sku', 'text', 'e.g. LAPTOP-PRO-15'],
              ['Product Name', 'productName', 'text', 'e.g. Laptop Pro 15"'],
              ['Description', 'description', 'text', 'Optional description'],
              ['Unit of Measure', 'unitOfMeasure', 'text', 'PCS, BOX, REAM…'],
              ['Cost Price (₹)', 'cost', 'number', '0.00'],
              ['List Price (₹)', 'listPrice', 'number', '0.00'],
            ].map(([label, key, type, ph]) => (
              <FormField key={key} label={label}>
                <input type={type} className="input" placeholder={ph} value={form[key]} onChange={e => setForm(f => ({ ...f, [key]: e.target.value }))} />
              </FormField>
            ))}
            <FormField label="Category">
              <select className="input" value={form.categoryId} onChange={e => setForm(f => ({ ...f, categoryId: e.target.value }))}>
                <option value="">— Select category —</option>
                {categories.map(c => <option key={c.categoryId} value={c.categoryId}>{c.categoryName}</option>)}
              </select>
            </FormField>
            <div style={{ display: 'flex', gap: '0.75rem', paddingTop: '0.5rem' }}>
              <button className="btn btn-default" style={{ flex: 1 }} onClick={handleSave} disabled={saving}>
                {saving ? 'Saving…' : modal === 'edit' ? 'Save Changes' : 'Create Product'}
              </button>
              <button className="btn btn-outline" onClick={() => setModal(null)}>Cancel</button>
            </div>
          </div>
        </Modal>
      )}

      {toast && <Toast message={toast.message} type={toast.type} onClose={() => setToast(null)} />}
    </div>
  )
}
