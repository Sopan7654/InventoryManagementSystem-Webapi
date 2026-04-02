// src/pages/StockLevels.jsx
import { useState, useEffect } from 'react'
import { Search, ArrowDownToLine, ArrowUpFromLine, ArrowLeftRight, SlidersHorizontal } from 'lucide-react'
import { getStockLevels, stockIn, stockOut, transfer, adjustment, getProducts, getWarehouses } from '../api/inventoryApi'
import PageHeader from '../components/PageHeader'
import Modal from '../components/Modal'
import Toast from '../components/Toast'
import LoadingSpinner from '../components/LoadingSpinner'

const modalTitles = {
  stockIn:    'Stock In',
  stockOut:   'Stock Out',
  transfer:   'Transfer Stock',
}

export default function StockLevels() {
  const [levels, setLevels]         = useState([])
  const [products, setProducts]     = useState([])
  const [warehouses, setWarehouses] = useState([])
  const [loading, setLoading]       = useState(true)
  const [modal, setModal]           = useState(null)
  const [form, setForm]             = useState({})
  const [toast, setToast]           = useState(null)
  const [saving, setSaving]         = useState(false)
  const [search, setSearch]         = useState('')

  const reload = () =>
    Promise.all([getStockLevels(), getProducts(), getWarehouses()])
      .then(([s, p, w]) => { setLevels(s.data || []); setProducts(p.data || []); setWarehouses(w.data || []) })
      .finally(() => setLoading(false))

  useEffect(() => { reload() }, [])

  const openModal = (type) => {
    setForm({ productId: '', warehouseId: '', toWarehouseId: '', quantity: '', reference: '' })
    setModal(type)
  }

  const handleSubmit = async () => {
    setSaving(true)
    try {
      const qty = parseFloat(form.quantity)
      if (!form.productId || !form.warehouseId || isNaN(qty) || qty <= 0)
        throw new Error('Please fill all required fields with valid values.')
      if (modal === 'stockIn')    await stockIn({ productId: form.productId, warehouseId: form.warehouseId, quantity: qty, reference: form.reference })
      if (modal === 'stockOut')   await stockOut({ productId: form.productId, warehouseId: form.warehouseId, quantity: qty, reference: form.reference })
      if (modal === 'transfer')   await transfer({ productId: form.productId, fromWarehouseId: form.warehouseId, toWarehouseId: form.toWarehouseId, quantity: qty })
      setToast({ message: 'Operation completed!', type: 'success' })
      setModal(null); reload()
    } catch (e) { setToast({ message: e.message, type: 'error' }) }
    finally { setSaving(false) }
  }

  const filtered = levels.filter(l =>
    l.productName?.toLowerCase().includes(search.toLowerCase()) ||
    l.warehouseName?.toLowerCase().includes(search.toLowerCase())
  )

  if (loading) return <LoadingSpinner />

  return (
    <div className="anim-fade-in">
      <PageHeader
        title="Stock Levels"
        subtitle="Current inventory across all warehouses"
        actions={
          <>
            <button className="btn btn-default btn-sm" onClick={() => openModal('stockIn')}>
              <ArrowDownToLine size={13} /> Stock In
            </button>
            <button className="btn btn-outline btn-sm" onClick={() => openModal('stockOut')}>
              <ArrowUpFromLine size={13} /> Stock Out
            </button>
            <button className="btn btn-outline btn-sm" onClick={() => openModal('transfer')}>
              <ArrowLeftRight size={13} /> Transfer
            </button>
          </>
        }
      />

      <div style={{ marginBottom: '1rem' }}>
        <div className="search-input-wrap" style={{ maxWidth: 300 }}>
          <Search size={15} className="search-icon-inner" />
          <input className="input" placeholder="Search product or warehouse…" value={search} onChange={e => setSearch(e.target.value)} />
        </div>
      </div>

      <div className="card" style={{ overflow: 'hidden' }}>
        <div className="data-table-wrap">
          <table className="data-table">
            <thead>
              <tr>
                <th>Product</th>
                <th>Warehouse</th>
                <th style={{ textAlign: 'right' }}>On Hand</th>
                <th style={{ textAlign: 'right' }}>Reserved</th>
                <th style={{ textAlign: 'right' }}>Available</th>
                <th style={{ textAlign: 'right' }}>Reorder Level</th>
                <th>Status</th>
              </tr>
            </thead>
            <tbody>
              {filtered.length === 0 && (
                <tr><td colSpan={7}><div className="empty-state"><span className="empty-icon">📦</span><p className="empty-title">No records found</p></div></td></tr>
              )}
              {filtered.map((l, i) => {
                const available = (l.quantityOnHand || 0) - (l.reservedQuantity || 0)
                const isLow = (l.quantityOnHand || 0) <= (l.reorderLevel || 0)
                return (
                  <tr key={i}>
                    <td style={{ fontWeight: 600, color: 'hsl(var(--foreground))' }}>{l.productName}</td>
                    <td>{l.warehouseName}</td>
                    <td style={{ textAlign: 'right' }} className="mono">{l.quantityOnHand?.toLocaleString()}</td>
                    <td style={{ textAlign: 'right', color: 'hsl(38 96% 55%)' }} className="mono">{l.reservedQuantity?.toLocaleString()}</td>
                    <td style={{ textAlign: 'right' }} className="mono">
                      <span style={{ color: available <= 0 ? 'hsl(var(--destructive))' : 'hsl(142 71% 45%)' }}>
                        {available.toLocaleString()}
                      </span>
                    </td>
                    <td style={{ textAlign: 'right' }} className="mono">{l.reorderLevel}</td>
                    <td>
                      {isLow
                        ? <span className="badge badge-danger">⚠ Low</span>
                        : <span className="badge badge-success">✓ OK</span>
                      }
                    </td>
                  </tr>
                )
              })}
            </tbody>
          </table>
        </div>
      </div>

      {modal && (
        <Modal title={modalTitles[modal]} onClose={() => setModal(null)}>
          <div style={{ display: 'flex', flexDirection: 'column', gap: '1rem' }}>
            <div>
              <label className="label">Product *</label>
              <select className="input" value={form.productId} onChange={e => setForm(f => ({ ...f, productId: e.target.value }))}>
                <option value="">— Select product —</option>
                {products.filter(p => p.isActive).map(p => <option key={p.productId} value={p.productId}>{p.productName} ({p.sku})</option>)}
              </select>
            </div>
            <div>
              <label className="label">{modal === 'transfer' ? 'From Warehouse *' : 'Warehouse *'}</label>
              <select className="input" value={form.warehouseId} onChange={e => setForm(f => ({ ...f, warehouseId: e.target.value }))}>
                <option value="">— Select warehouse —</option>
                {warehouses.map(w => <option key={w.warehouseId} value={w.warehouseId}>{w.warehouseName}</option>)}
              </select>
            </div>
            {modal === 'transfer' && (
              <div>
                <label className="label">To Warehouse *</label>
                <select className="input" value={form.toWarehouseId} onChange={e => setForm(f => ({ ...f, toWarehouseId: e.target.value }))}>
                  <option value="">— Select warehouse —</option>
                  {warehouses.map(w => <option key={w.warehouseId} value={w.warehouseId}>{w.warehouseName}</option>)}
                </select>
              </div>
            )}
            <div>
              <label className="label">Quantity *</label>
              <input type="number" className="input" value={form.quantity} onChange={e => setForm(f => ({ ...f, quantity: e.target.value }))} placeholder="0" />
            </div>
            <div>
              <label className="label">Reference / Reason</label>
              <input type="text" className="input" value={form.reference} onChange={e => setForm(f => ({ ...f, reference: e.target.value }))} placeholder="PO-123, count adjustment…" />
            </div>
            <div style={{ display: 'flex', gap: '0.75rem', paddingTop: '0.25rem' }}>
              <button className="btn btn-default" style={{ flex: 1 }} onClick={handleSubmit} disabled={saving}>
                {saving ? 'Processing…' : 'Confirm'}
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
