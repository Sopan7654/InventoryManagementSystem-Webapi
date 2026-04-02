// src/pages/Categories.jsx
import { useState, useEffect } from 'react'
import { Plus, Pencil, FolderOpen } from 'lucide-react'
import { getCategories, createCategory, updateCategory } from '../api/inventoryApi'
import PageHeader from '../components/PageHeader'
import Modal from '../components/Modal'
import Toast from '../components/Toast'
import LoadingSpinner from '../components/LoadingSpinner'

const EMPTY = { categoryName: '', description: '', parentCategoryId: '' }

export default function Categories() {
  const [records, setRecords] = useState([])
  const [loading, setLoading] = useState(true)
  const [modal, setModal]     = useState(null)
  const [form, setForm]       = useState(EMPTY)
  const [editId, setEditId]   = useState(null)
  const [toast, setToast]     = useState(null)
  const [saving, setSaving]   = useState(false)

  const reload = () => getCategories().then(r => setRecords(r.data || [])).finally(() => setLoading(false))
  useEffect(() => { reload() }, [])

  const openCreate = () => { setForm(EMPTY); setEditId(null); setModal('form') }
  const openEdit = (c) => {
    setForm({ categoryName: c.categoryName, description: c.description || '', parentCategoryId: c.parentCategoryId || '' })
    setEditId(c.categoryId); setModal('form')
  }

  const handleSave = async () => {
    setSaving(true)
    try {
      if (editId) await updateCategory(editId, form)
      else        await createCategory(form)
      setToast({ message: editId ? 'Category updated!' : 'Category created!', type: 'success' })
      setModal(null); reload()
    } catch (e) { setToast({ message: e.message, type: 'error' }) }
    finally { setSaving(false) }
  }

  const children = records.filter(r => r.parentCategoryId)
  if (loading) return <LoadingSpinner />

  return (
    <div className="anim-fade-in">
      <PageHeader
        title="Product Categories"
        subtitle={`${records.length} categories`}
        actions={
          <button className="btn btn-default" onClick={openCreate}>
            <Plus size={15} strokeWidth={2.5} /> Add Category
          </button>
        }
      />

      <div className="card" style={{ overflow: 'hidden' }}>
        <div className="data-table-wrap">
          <table className="data-table">
            <thead>
              <tr>
                <th>Name</th>
                <th>Description</th>
                <th>Parent</th>
                <th>Sub-categories</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              {records.length === 0 && (
                <tr><td colSpan={5}><div className="empty-state"><span className="empty-icon">🗂️</span><p className="empty-title">No categories yet</p></div></td></tr>
              )}
              {records.map(c => (
                <tr key={c.categoryId}>
                  <td style={{ fontWeight: 600, color: 'hsl(var(--foreground))' }}>
                    {c.parentCategoryId && <span style={{ color: 'hsl(var(--muted-foreground))', marginRight: '0.375rem' }}>↳</span>}
                    <FolderOpen size={13} style={{ display: 'inline', marginRight: '0.375rem', color: 'hsl(var(--primary))' }} />
                    {c.categoryName}
                  </td>
                  <td>{c.description || <span style={{ opacity: 0.4 }}>—</span>}</td>
                  <td>
                    {c.parentCategoryName
                      ? c.parentCategoryName
                      : <span className="badge badge-default">Root</span>
                    }
                  </td>
                  <td>
                    <span className="badge badge-secondary">
                      {children.filter(ch => ch.parentCategoryId === c.categoryId).length}
                    </span>
                  </td>
                  <td>
                    <button className="btn btn-outline btn-sm" onClick={() => openEdit(c)}>
                      <Pencil size={12} /> Edit
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>

      {modal && (
        <Modal title={editId ? 'Edit Category' : 'New Category'} onClose={() => setModal(null)}>
          <div style={{ display: 'flex', flexDirection: 'column', gap: '1rem' }}>
            <div>
              <label className="label">Name *</label>
              <input className="input" value={form.categoryName} onChange={e => setForm(f => ({ ...f, categoryName: e.target.value }))} placeholder="e.g. Electronics" />
            </div>
            <div>
              <label className="label">Description</label>
              <input className="input" value={form.description} onChange={e => setForm(f => ({ ...f, description: e.target.value }))} placeholder="Optional description" />
            </div>
            <div>
              <label className="label">Parent Category</label>
              <select className="input" value={form.parentCategoryId} onChange={e => setForm(f => ({ ...f, parentCategoryId: e.target.value }))}>
                <option value="">— Root category —</option>
                {records.filter(r => r.categoryId !== editId).map(r => <option key={r.categoryId} value={r.categoryId}>{r.categoryName}</option>)}
              </select>
            </div>
            <div style={{ display: 'flex', gap: '0.75rem' }}>
              <button className="btn btn-default" style={{ flex: 1 }} onClick={handleSave} disabled={saving}>
                {saving ? 'Saving…' : editId ? 'Save Changes' : 'Create'}
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
