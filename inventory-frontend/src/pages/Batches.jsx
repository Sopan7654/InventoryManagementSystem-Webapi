// src/pages/Batches.jsx
import { useState, useEffect } from 'react'
import { Plus, Search } from 'lucide-react'
import { getBatches, createBatch, getProducts, getWarehouses } from '../api/inventoryApi'
import PageHeader from '../components/PageHeader'
import Modal from '../components/Modal'
import Toast from '../components/Toast'
import LoadingSpinner from '../components/LoadingSpinner'

const EMPTY = { productId: '', warehouseId: '', batchNumber: '', quantity: '', manufacturingDate: '', expiryDate: '' }

const statusMap = {
  ACTIVE:        'badge-success',
  EXPIRED:       'badge-danger',
  EXPIRING_SOON: 'badge-warning',
  NO_EXPIRY:     'badge-secondary',
}

export default function Batches() {
  const [records, setRecords]   = useState([])
  const [products, setProducts] = useState([])
  const [loading, setLoading]   = useState(true)
  const [modal, setModal]       = useState(false)
  const [form, setForm]         = useState(EMPTY)
  const [toast, setToast]       = useState(null)
  const [saving, setSaving]     = useState(false)
  const [search, setSearch]     = useState('')

  const [warehouses, setWarehouses] = useState([])

  const reload = () =>
    Promise.all([getBatches(), getProducts(), getWarehouses()])
      .then(([b, p, w]) => { 
        setRecords(b.data || []); 
        setProducts(p.data || []);
        setWarehouses(w.data || []);
      })
      .finally(() => setLoading(false))

  useEffect(() => { reload() }, [])

  const handleSave = async () => {
    setSaving(true)
    try {
      await createBatch(form)
      setToast({ message: 'Batch created!', type: 'success' })
      setModal(false); setForm(EMPTY); reload()
    } catch (e) { setToast({ message: e.message, type: 'error' }) }
    finally { setSaving(false) }
  }

  const filtered = records.filter(r =>
    r.productName?.toLowerCase().includes(search.toLowerCase()) ||
    r.batchNumber?.toLowerCase().includes(search.toLowerCase())
  )

  if (loading) return <LoadingSpinner />

  return (
    <div className="anim-fade-in">
      <PageHeader
        title="Batch / Lot Tracking"
        subtitle={`${records.length} batches`}
        actions={
          <button className="btn btn-default" onClick={() => setModal(true)}>
            <Plus size={15} strokeWidth={2.5} /> New Batch
          </button>
        }
      />

      <div style={{ marginBottom: '1rem' }}>
        <div className="search-input-wrap" style={{ maxWidth: 300 }}>
          <Search size={15} className="search-icon-inner" />
          <input className="input" placeholder="Search batches…" value={search} onChange={e => setSearch(e.target.value)} />
        </div>
      </div>

      <div className="card" style={{ overflow: 'hidden' }}>
        <div className="data-table-wrap">
          <table className="data-table">
            <thead>
              <tr>
                <th>Batch #</th>
                <th>Product</th>
                <th style={{ textAlign: 'right' }}>Quantity</th>
                <th>Mfg. Date</th>
                <th>Expiry Date</th>
                <th>Status</th>
              </tr>
            </thead>
            <tbody>
              {filtered.length === 0 && (
                <tr><td colSpan={6}><div className="empty-state"><span className="empty-icon">🧪</span><p className="empty-title">No batches found</p></div></td></tr>
              )}
              {filtered.map(b => (
                <tr key={b.batchId}>
                  <td className="mono" style={{ color: 'hsl(var(--primary))', fontSize: '0.8125rem', fontWeight: 600 }}>{b.batchNumber}</td>
                  <td style={{ fontWeight: 600, color: 'hsl(var(--foreground))' }}>{b.productName}</td>
                  <td style={{ textAlign: 'right' }} className="mono">{b.quantity?.toLocaleString()}</td>
                  <td style={{ fontSize: '0.8125rem' }}>{b.manufacturingDate?.slice(0, 10) || '—'}</td>
                  <td style={{ fontSize: '0.8125rem' }}>{b.expiryDate?.slice(0, 10) || 'N/A'}</td>
                  <td><span className={`badge ${statusMap[b.expiryStatus] || 'badge-secondary'}`}>{b.expiryStatus?.replace('_', ' ')}</span></td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>

      {modal && (
        <Modal title="New Batch" onClose={() => setModal(false)}>
          <div style={{ display: 'flex', flexDirection: 'column', gap: '1rem' }}>
            <div>
              <label className="label">Product *</label>
              <select className="input" value={form.productId} onChange={e => setForm(f => ({ ...f, productId: e.target.value }))}>
                <option value="">— Select product —</option>
                {products.filter(p => p.isActive).map(p => <option key={p.productId} value={p.productId}>{p.productName}</option>)}
              </select>
            </div>
            <div>
              <label className="label">Warehouse *</label>
              <select className="input" value={form.warehouseId} onChange={e => setForm(f => ({ ...f, warehouseId: e.target.value }))}>
                <option value="">— Select warehouse —</option>
                {warehouses.map(w => <option key={w.warehouseId} value={w.warehouseId}>{w.warehouseName}</option>)}
              </select>
            </div>
            {[['Batch Number *', 'batchNumber', 'text', 'BATCH-2024-001'], ['Quantity *', 'quantity', 'number', '100'], ['Manufacturing Date', 'manufacturingDate', 'date', ''], ['Expiry Date', 'expiryDate', 'date', '']].map(([label, key, type, ph]) => (
              <div key={key}>
                <label className="label">{label}</label>
                <input type={type} className="input" placeholder={ph} value={form[key]} onChange={e => setForm(f => ({ ...f, [key]: e.target.value }))} />
              </div>
            ))}
            <div style={{ display: 'flex', gap: '0.75rem' }}>
              <button className="btn btn-default" style={{ flex: 1 }} onClick={handleSave} disabled={saving}>
                {saving ? 'Saving…' : 'Create Batch'}
              </button>
              <button className="btn btn-outline" onClick={() => setModal(false)}>Cancel</button>
            </div>
          </div>
        </Modal>
      )}
      {toast && <Toast message={toast.message} type={toast.type} onClose={() => setToast(null)} />}
    </div>
  )
}
