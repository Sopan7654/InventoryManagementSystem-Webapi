// src/pages/Transactions.jsx
import { useState, useEffect } from 'react'
import { Filter } from 'lucide-react'
import { getTransactions, getProducts } from '../api/inventoryApi'
import PageHeader from '../components/PageHeader'
import LoadingSpinner from '../components/LoadingSpinner'

const typeBadge = {
  PURCHASE:     'badge-success',
  SALE:         'badge-danger',
  TRANSFER_IN:  'badge-default',
  TRANSFER_OUT: 'badge-warning',
  ADJUSTMENT:   'badge-secondary',
  RETURN:       'badge-default',
}

export default function Transactions() {
  const [records, setRecords]     = useState([])
  const [products, setProducts]   = useState([])
  const [loading, setLoading]     = useState(true)
  const [productId, setProductId] = useState('')
  const [limit, setLimit]         = useState(50)

  const reload = () => {
    setLoading(true)
    Promise.all([getTransactions(productId || undefined, limit), getProducts()])
      .then(([t, p]) => { setRecords(t.data || []); setProducts(p.data || []) })
      .finally(() => setLoading(false))
  }

  useEffect(() => { reload() }, [productId, limit])

  return (
    <div className="anim-fade-in">
      <PageHeader title="Transaction History" subtitle={`${records.length} transactions shown`} />

      {/* Filters */}
      <div style={{ display: 'flex', gap: '0.75rem', marginBottom: '1rem', flexWrap: 'wrap', alignItems: 'center' }}>
        <Filter size={15} style={{ color: 'hsl(var(--muted-foreground))' }} />
        <select className="input" style={{ width: 220 }} value={productId} onChange={e => setProductId(e.target.value)}>
          <option value="">All Products</option>
          {products.map(p => <option key={p.productId} value={p.productId}>{p.productName}</option>)}
        </select>
        <select className="input" style={{ width: 120 }} value={limit} onChange={e => setLimit(Number(e.target.value))}>
          {[25, 50, 100, 200].map(n => <option key={n} value={n}>{n} rows</option>)}
        </select>
      </div>

      {loading ? <LoadingSpinner /> : (
        <div className="card" style={{ overflow: 'hidden' }}>
          <div className="data-table-wrap">
            <table className="data-table">
              <thead>
                <tr>
                  <th>Date &amp; Time</th>
                  <th>Product</th>
                  <th>Warehouse</th>
                  <th>Type</th>
                  <th style={{ textAlign: 'right' }}>Quantity</th>
                  <th>Reference</th>
                </tr>
              </thead>
              <tbody>
                {records.length === 0 && (
                  <tr><td colSpan={6}><div className="empty-state"><span className="empty-icon">🔄</span><p className="empty-title">No transactions found</p></div></td></tr>
                )}
                {records.map((r, i) => (
                  <tr key={i}>
                    <td className="mono" style={{ fontSize: '0.75rem' }}>{r.transactionDate?.replace('T', ' ').slice(0, 16)}</td>
                    <td style={{ fontWeight: 600, color: 'hsl(var(--foreground))' }}>{r.productName}</td>
                    <td>{r.warehouseName}</td>
                    <td><span className={`badge ${typeBadge[r.transactionType] || 'badge-secondary'}`}>{r.transactionType?.replace('_', ' ')}</span></td>
                    <td style={{ textAlign: 'right' }} className="mono">
                      <span style={{ color: ['SALE', 'TRANSFER_OUT'].includes(r.transactionType) ? 'hsl(var(--destructive))' : 'hsl(142 71% 45%)' }}>
                        {['SALE', 'TRANSFER_OUT'].includes(r.transactionType) ? '−' : '+'}{r.quantity}
                      </span>
                    </td>
                    <td style={{ fontSize: '0.75rem', opacity: 0.7 }}>{r.reference || '—'}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      )}
    </div>
  )
}
