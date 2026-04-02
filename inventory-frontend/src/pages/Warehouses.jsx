// src/pages/Warehouses.jsx
import { useState, useEffect } from 'react'
import { Plus, Pencil, MapPin, Boxes } from 'lucide-react'
import { getWarehouses, createWarehouse, updateWarehouse } from '../api/inventoryApi'
import PageHeader from '../components/PageHeader'
import Modal from '../components/Modal'
import Toast from '../components/Toast'
import LoadingSpinner from '../components/LoadingSpinner'

const EMPTY = { warehouseName: '', location: '', capacity: '' }

export default function Warehouses() {
  const [records, setRecords] = useState([])
  const [loading, setLoading] = useState(true)
  const [modal, setModal]     = useState(null)
  const [form, setForm]       = useState(EMPTY)
  const [editId, setEditId]   = useState(null)
  const [toast, setToast]     = useState(null)
  const [saving, setSaving]   = useState(false)

  const reload = () => getWarehouses().then(r => setRecords(r.data || [])).finally(() => setLoading(false))
  useEffect(() => { reload() }, [])

  const openCreate = () => { setForm(EMPTY); setEditId(null); setModal('form') }
  const openEdit = (w) => {
    setForm({ warehouseName: w.warehouseName, location: w.location || '', capacity: w.capacity || '' })
    setEditId(w.warehouseId); setModal('form')
  }

  const handleSave = async () => {
    if (!form.warehouseName.trim()) {
      setToast({ message: 'Warehouse name is required.', type: 'error' })
      return
    }
    setSaving(true)
    try {
      const payload = {
        warehouseName: form.warehouseName.trim(),
        location:      form.location.trim() || null,
        capacity:      form.capacity !== '' ? parseFloat(form.capacity) : null,
      }
      if (editId) await updateWarehouse(editId, payload)
      else        await createWarehouse(payload)
      setToast({ message: editId ? 'Warehouse updated!' : 'Warehouse created!', type: 'success' })
      setModal(null); reload()
    } catch (e) { setToast({ message: e.message, type: 'error' }) }
    finally { setSaving(false) }
  }


  if (loading) return <LoadingSpinner />

  const gradients = [
    'linear-gradient(135deg, #6366f1, #8b5cf6)',
    'linear-gradient(135deg, #06b6d4, #3b82f6)',
    'linear-gradient(135deg, #10b981, #06b6d4)',
    'linear-gradient(135deg, #f59e0b, #ef4444)',
    'linear-gradient(135deg, #ec4899, #8b5cf6)',
    'linear-gradient(135deg, #84cc16, #10b981)',
  ]

  return (
    <div className="anim-fade-in">
      <PageHeader
        title="Warehouses"
        subtitle={`${records.length} locations`}
        actions={
          <button className="btn btn-default" onClick={openCreate}>
            <Plus size={15} strokeWidth={2.5} /> Add Warehouse
          </button>
        }
      />

      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fill, minmax(260px, 1fr))', gap: '1rem' }}>
        {records.length === 0 && (
          <div className="card" style={{ gridColumn: '1/-1' }}>
            <div className="empty-state"><span className="empty-icon">🏢</span><p className="empty-title">No warehouses yet</p></div>
          </div>
        )}
        {records.map((w, i) => (
          <div key={w.warehouseId} className="card card-hover" style={{ padding: '1.25rem' }}>
            <div style={{ display: 'flex', alignItems: 'flex-start', justifyContent: 'space-between', marginBottom: '1rem' }}>
              <div style={{
                width: 44, height: 44, borderRadius: 12,
                background: gradients[i % gradients.length],
                display: 'flex', alignItems: 'center', justifyContent: 'center',
                boxShadow: '0 4px 14px rgba(0,0,0,0.2)',
              }}>
                <Boxes size={20} color="#fff" strokeWidth={1.75} />
              </div>
              <button className="btn btn-outline btn-sm" onClick={() => openEdit(w)}>
                <Pencil size={12} /> Edit
              </button>
            </div>
            <p style={{ fontSize: '1rem', fontWeight: 700, color: 'hsl(var(--foreground))' }}>{w.warehouseName}</p>
            {w.location && (
              <p style={{ fontSize: '0.8125rem', color: 'hsl(var(--muted-foreground))', marginTop: '0.25rem', display: 'flex', alignItems: 'center', gap: '0.375rem' }}>
                <MapPin size={12} /> {w.location}
              </p>
            )}
            {w.capacity && (
              <div style={{ marginTop: '0.875rem', paddingTop: '0.875rem', borderTop: '1px solid hsl(var(--border))', display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
                <span style={{ fontSize: '0.75rem', color: 'hsl(var(--muted-foreground))' }}>Capacity</span>
                <span style={{ fontSize: '0.8125rem', fontWeight: 600, fontFamily: "'JetBrains Mono', monospace" }}>
                  {Number(w.capacity).toLocaleString()} units
                </span>
              </div>
            )}
          </div>
        ))}
      </div>

      {modal && (
        <Modal title={editId ? 'Edit Warehouse' : 'New Warehouse'} onClose={() => setModal(null)}>
          <div style={{ display: 'flex', flexDirection: 'column', gap: '1rem' }}>
            {[['Name *', 'warehouseName', 'text', 'e.g. Main Warehouse'], ['Location', 'location', 'text', 'City, State'], ['Capacity (units)', 'capacity', 'number', '5000']].map(([label, key, type, ph]) => (
              <div key={key}>
                <label className="label">{label}</label>
                <input type={type} className="input" placeholder={ph} value={form[key]} onChange={e => setForm(f => ({ ...f, [key]: e.target.value }))} />
              </div>
            ))}
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
