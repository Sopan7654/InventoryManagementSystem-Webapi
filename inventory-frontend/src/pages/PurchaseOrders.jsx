// src/pages/PurchaseOrders.jsx
import { useState, useEffect } from 'react'
import { Plus, PackageCheck, Trash2 } from 'lucide-react'
import { getPurchaseOrders, createPurchaseOrder, receivePurchaseOrder, getSuppliers, getProducts, getPurchaseOrderItems, getWarehouses } from '../api/inventoryApi'
import PageHeader from '../components/PageHeader'
import Modal from '../components/Modal'
import Toast from '../components/Toast'
import LoadingSpinner from '../components/LoadingSpinner'

const statusBadge = {
  PENDING:   'badge-warning',
  RECEIVED:  'badge-success',
  CANCELLED: 'badge-danger',
  PARTIAL:   'badge-default',
}

export default function PurchaseOrders() {
  const [records, setRecords]     = useState([])
  const [suppliers, setSuppliers] = useState([])
  const [products, setProducts]   = useState([])
  const [loading, setLoading]     = useState(true)
  const [modal, setModal]         = useState(null)
  const [form, setForm]           = useState({ supplierId: '', items: [{ productId: '', quantityOrdered: '', unitPrice: '' }] })
  const [toast, setToast]         = useState(null)
  const [saving, setSaving]       = useState(false)
  const [poItems, setPoItems]     = useState([])
  const [warehouses, setWarehouses] = useState([])
  const [receivePoId, setReceivePoId] = useState(null)
  const [receiveWarehouse, setReceiveWarehouse] = useState('')

  const reload = () =>
    Promise.all([getPurchaseOrders(), getSuppliers(), getProducts(), getWarehouses()])
      .then(([po, s, p, w]) => { 
        setRecords(po.data || []); 
        setSuppliers(s.data || []); 
        setProducts(p.data || []);
        setWarehouses(w.data || []);
      })
      .finally(() => setLoading(false))

  useEffect(() => { reload() }, [])

  const addItem    = () => setForm(f => ({ ...f, items: [...f.items, { productId: '', quantityOrdered: '', unitPrice: '' }] }))
  const removeItem = (i) => setForm(f => ({ ...f, items: f.items.filter((_, idx) => idx !== i) }))
  const updateItem = (i, key, val) => setForm(f => ({ ...f, items: f.items.map((it, idx) => idx === i ? { ...it, [key]: val } : it) }))

  const handleCreate = async () => {
    setSaving(true)
    try {
      await createPurchaseOrder(form)
      setToast({ message: 'Purchase order created!', type: 'success' })
      setModal(null); reload()
    } catch (e) { setToast({ message: e.message, type: 'error' }) }
    finally { setSaving(false) }
  }

  const handleOpenReceive = (id) => {
    setReceivePoId(id)
    setReceiveWarehouse('')
    setModal('receive')
  }

  const handleReceiveConfirm = async () => {
    if (!receiveWarehouse) { setToast({ message: 'Please select a warehouse', type: 'error' }); return; }
    setSaving(true)
    try {
      await receivePurchaseOrder(receivePoId, receiveWarehouse)
      setToast({ message: 'PO received! Stock updated.', type: 'success' })
      setModal(null); reload()
    } catch (e) { setToast({ message: e.message, type: 'error' }) }
    finally { setSaving(false) }
  }

  const handleViewItems = async (id) => {
    try {
      const res = await getPurchaseOrderItems(id);
      setPoItems(res.data || []);
      setModal('view');
    } catch (e) { setToast({ message: e.message, type: 'error' }) }
  }

  if (loading) return <LoadingSpinner />

  return (
    <div className="anim-fade-in">
      <PageHeader
        title="Purchase Orders"
        subtitle={`${records.length} orders`}
        actions={
          <button className="btn btn-default" onClick={() => setModal('create')}>
            <Plus size={15} strokeWidth={2.5} /> New PO
          </button>
        }
      />

      <div className="card" style={{ overflow: 'hidden' }}>
        <div className="data-table-wrap">
          <table className="data-table">
            <thead>
              <tr>
                <th>PO ID</th>
                <th>Supplier</th>
                <th>Date</th>
                <th>Status</th>
                <th style={{ textAlign: 'right' }}>Items</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              {records.length === 0 && (
                <tr><td colSpan={6}><div className="empty-state"><span className="empty-icon">📋</span><p className="empty-title">No purchase orders yet</p></div></td></tr>
              )}
              {records.map(po => (
                <tr key={po.purchaseOrderId}>
                  <td className="mono" style={{ color: 'hsl(var(--primary))', fontSize: '0.75rem', fontWeight: 600 }}>
                    {po.purchaseOrderId?.slice(0, 12)}…
                  </td>
                  <td style={{ fontWeight: 600, color: 'hsl(var(--foreground))' }}>{po.supplierName || po.supplierId}</td>
                  <td style={{ fontSize: '0.8125rem' }}>{po.orderDate?.slice(0, 10)}</td>
                  <td><span className={`badge ${statusBadge[po.status] || 'badge-secondary'}`}>{po.status}</span></td>
                  <td style={{ textAlign: 'right' }} className="mono">{po.itemCount ?? '—'}</td>
                  <td>
                    <button className="btn btn-ghost btn-sm" onClick={() => handleViewItems(po.purchaseOrderId)}>
                      View Items
                    </button>
                    {['PENDING', 'APPROVED'].includes(po.status) && (
                      <button className="btn btn-outline btn-sm" onClick={() => handleOpenReceive(po.purchaseOrderId)} style={{ marginLeft: '0.5rem' }}>
                        <PackageCheck size={13} /> Receive
                      </button>
                    )}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>

      {modal === 'create' && (
        <Modal title="New Purchase Order" onClose={() => setModal(null)}>
          <div style={{ display: 'flex', flexDirection: 'column', gap: '1rem' }}>
            <div>
              <label className="label">Supplier *</label>
              <select className="input" value={form.supplierId} onChange={e => setForm(f => ({ ...f, supplierId: e.target.value }))}>
                <option value="">— Select supplier —</option>
                {suppliers.map(s => <option key={s.supplierId} value={s.supplierId}>{s.supplierName}</option>)}
              </select>
            </div>

            <div>
              <label className="label" style={{ marginBottom: '0.625rem' }}>Line Items</label>
              {form.items.map((item, i) => (
                <div key={i} style={{ display: 'flex', gap: '0.5rem', marginBottom: '0.5rem', alignItems: 'center' }}>
                  <select className="input" style={{ flex: 2 }} value={item.productId} onChange={e => updateItem(i, 'productId', e.target.value)}>
                    <option value="">Product</option>
                    {products.filter(p => p.isActive).map(p => <option key={p.productId} value={p.productId}>{p.productName}</option>)}
                  </select>
                  <input type="number" className="input" style={{ width: 80 }} placeholder="Qty" value={item.quantityOrdered} onChange={e => updateItem(i, 'quantityOrdered', e.target.value)} />
                  <input type="number" className="input" style={{ width: 100 }} placeholder="Unit ₹" value={item.unitPrice} onChange={e => updateItem(i, 'unitPrice', e.target.value)} />
                  {i > 0 && (
                    <button onClick={() => removeItem(i)} className="btn btn-destructive" style={{ padding: '0.4rem 0.5rem', borderRadius: '6px', flexShrink: 0 }}>
                      <Trash2 size={13} />
                    </button>
                  )}
                </div>
              ))}
              <button onClick={addItem} className="btn btn-ghost btn-sm" style={{ marginTop: '0.25rem' }}>
                <Plus size={13} /> Add Item
              </button>
            </div>

            <div style={{ display: 'flex', gap: '0.75rem' }}>
              <button className="btn btn-default" style={{ flex: 1 }} onClick={handleCreate} disabled={saving}>
                {saving ? 'Creating…' : 'Create PO'}
              </button>
              <button className="btn btn-outline" onClick={() => setModal(null)}>Cancel</button>
            </div>
          </div>
        </Modal>
      )}

      {modal === 'view' && (
        <Modal title="Purchase Order Items" onClose={() => setModal(null)}>
          <div style={{ maxHeight: '400px', overflowY: 'auto' }}>
            <table className="data-table">
              <thead>
                <tr>
                  <th>Product</th>
                  <th style={{ textAlign: 'right' }}>Qty</th>
                  <th style={{ textAlign: 'right' }}>Unit Price (₹)</th>
                  <th style={{ textAlign: 'right' }}>Total (₹)</th>
                </tr>
              </thead>
              <tbody>
                {poItems.length === 0 ? (
                  <tr><td colSpan={4} style={{ textAlign: 'center' }}>No items found.</td></tr>
                ) : poItems.map(item => (
                  <tr key={item.poItemId}>
                    <td style={{ fontWeight: 600 }}>{item.productName}</td>
                    <td style={{ textAlign: 'right' }} className="mono">{item.quantityOrdered}</td>
                    <td style={{ textAlign: 'right' }} className="mono">{item.unitPrice}</td>
                    <td style={{ textAlign: 'right', fontWeight: 600, color: 'hsl(var(--primary))' }} className="mono">
                      {(item.quantityOrdered * item.unitPrice).toLocaleString()}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
          <div style={{ display: 'flex', justifyContent: 'flex-end', marginTop: '1rem' }}>
            <button className="btn btn-outline" onClick={() => setModal(null)}>Close</button>
          </div>
        </Modal>
      )}

      {modal === 'receive' && (
        <Modal title="Receive Purchase Order" onClose={() => setModal(null)}>
          <div style={{ display: 'flex', flexDirection: 'column', gap: '1rem' }}>
            <div>
              <label className="label">Destination Warehouse *</label>
              <select className="input" value={receiveWarehouse} onChange={e => setReceiveWarehouse(e.target.value)}>
                <option value="">— Select warehouse to receive stock —</option>
                {warehouses.map(w => <option key={w.warehouseId} value={w.warehouseId}>{w.warehouseName}</option>)}
              </select>
            </div>
            
            <p style={{ fontSize: '0.875rem', color: 'hsl(var(--muted-foreground))' }}>
              Marking this PO as received will automatically insert all line items into the selected warehouse's stock levels. This action is irreversible.
            </p>

            <div style={{ display: 'flex', gap: '0.75rem', marginTop: '0.5rem' }}>
              <button className="btn btn-default" style={{ flex: 1 }} onClick={handleReceiveConfirm} disabled={saving || !receiveWarehouse}>
                {saving ? 'Processing…' : 'Confirm & Receive'}
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
