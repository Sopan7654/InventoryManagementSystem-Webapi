// src/pages/Reports.jsx
import { useState, useEffect } from 'react'
import { BarChart2, PieChart as PieIcon, Activity, Clock, TrendingUp, ArrowLeftRight } from 'lucide-react'
import {
  BarChart, Bar, PieChart, Pie, Cell, XAxis, YAxis, Tooltip,
  ResponsiveContainer, CartesianGrid, Legend,
} from 'recharts'
import { getLowStock, getOverstock, getAbcAnalysis, getStockAging, getTurnover, getTransactions } from '../api/inventoryApi'
import PageHeader from '../components/PageHeader'
import LoadingSpinner from '../components/LoadingSpinner'
import { cn } from '../lib/utils'

const TABS = [
  { id: 'Low Stock',    Icon: Activity,      label: 'Low Stock' },
  { id: 'Overstock',    Icon: BarChart2,      label: 'Overstock' },
  { id: 'ABC Analysis', Icon: PieIcon,        label: 'ABC Analysis' },
  { id: 'Stock Aging',  Icon: Clock,          label: 'Stock Aging' },
  { id: 'Turnover',     Icon: TrendingUp,     label: 'Turnover' },
  { id: 'Transactions', Icon: ArrowLeftRight, label: 'Transactions' },
]

const PALETTE = ['#6366f1', '#8b5cf6', '#a78bfa', '#c4b5fd', '#e879f9', '#f0abfc']

// ── Safe array helper — always returns an array ──────────────────────────────
const safeArr = (v) => (Array.isArray(v) ? v : [])

export default function Reports() {
  const [tab, setTab]           = useState('Low Stock')
  const [data, setData]         = useState(null)
  const [loading, setLoading]   = useState(false)
  const [error, setError]       = useState(null)
  const [dateFrom, setDateFrom] = useState(new Date(Date.now() - 30 * 86400000).toISOString().slice(0, 10))
  const [dateTo, setDateTo]     = useState(new Date().toISOString().slice(0, 10))

  useEffect(() => {
    setData(null); setError(null); setLoading(true)
    const loaders = {
      'Low Stock':    () => getLowStock(),
      'Overstock':    () => getOverstock(),
      'ABC Analysis': () => getAbcAnalysis(),
      'Stock Aging':  () => getStockAging(),
      'Turnover':     () => getTurnover(dateFrom, dateTo),
      'Transactions': () => getTransactions(),
    }
    loaders[tab]()
      .then(r  => setData(r))
      .catch(e => setError(e.message))
      .finally(() => setLoading(false))
  }, [tab, dateFrom, dateTo])

  return (
    <div className="anim-fade-in">
      <PageHeader title="Reports & Analytics" subtitle="Business intelligence for your inventory" />

      {/* Tab bar */}
      <div style={{
        display: 'flex', gap: '0.25rem', marginBottom: '1.5rem',
        background: 'hsl(var(--secondary))', padding: '4px', borderRadius: '10px',
        border: '1px solid hsl(var(--border))', width: 'fit-content', flexWrap: 'wrap',
      }}>
        {TABS.map(({ id, Icon, label }) => (
          <button key={id} onClick={() => setTab(id)} style={{
            display: 'flex', alignItems: 'center', gap: '0.375rem',
            padding: '0.4rem 0.875rem', borderRadius: '7px',
            fontFamily: 'inherit', fontSize: '0.8125rem', fontWeight: 500,
            cursor: 'pointer', border: 'none', transition: 'all 0.15s',
            background: tab === id ? 'hsl(var(--primary))' : 'transparent',
            color: tab === id ? '#fff' : 'hsl(var(--muted-foreground))',
            boxShadow: tab === id ? '0 2px 8px hsl(var(--primary) / 0.3)' : 'none',
          }}>
            <Icon size={13} strokeWidth={2} />
            {label}
          </button>
        ))}
      </div>

      {/* Date filter for Turnover */}
      {tab === 'Turnover' && (
        <div style={{ display: 'flex', gap: '0.75rem', marginBottom: '1rem', alignItems: 'flex-end' }}>
          <div>
            <label className="label">From</label>
            <input type="date" className="input" style={{ width: 160 }} value={dateFrom} onChange={e => setDateFrom(e.target.value)} />
          </div>
          <div>
            <label className="label">To</label>
            <input type="date" className="input" style={{ width: 160 }} value={dateTo} onChange={e => setDateTo(e.target.value)} />
          </div>
        </div>
      )}

      {/* Loading */}
      {loading && <LoadingSpinner />}

      {/* Error */}
      {!loading && error && (
        <div className="card" style={{ padding: '2rem', textAlign: 'center', color: 'hsl(var(--destructive))' }}>
          ⚠ {error}
        </div>
      )}

      {/* Content — only render when data is ready and no error */}
      {!loading && !error && data && (
        <>
          {tab === 'Low Stock'    && <LowStockView    data={safeArr(data.data)} />}
          {tab === 'Overstock'    && <OverstockView   data={safeArr(data.data)} />}
          {tab === 'ABC Analysis' && <AbcView         data={data.data} />}
          {tab === 'Stock Aging'  && <AgingView       data={data.data} />}
          {tab === 'Turnover'     && <TurnoverView    data={safeArr(data.data)} />}
          {tab === 'Transactions' && <TransactionsView data={safeArr(data.data)} />}
        </>
      )}
    </div>
  )
}

/* ── SHARED ── */

function ReportTable({ children }) {
  return (
    <div className="card" style={{ overflow: 'hidden' }}>
      <div className="data-table-wrap">
        <table className="data-table">{children}</table>
      </div>
    </div>
  )
}

function EmptyState({ icon, text }) {
  return (
    <tr>
      <td colSpan={99}>
        <div className="empty-state">
          <span className="empty-icon">{icon}</span>
          <p className="empty-title">{text}</p>
        </div>
      </td>
    </tr>
  )
}

/* ── LOW STOCK ── */
function LowStockView({ data }) {
  const chartData = data.slice(0, 8).map(r => ({
    name: (r.productName || '').slice(0, 12),
    onHand: r.quantityOnHand ?? 0,
    reorderLevel: r.reorderLevel ?? 0,
  }))

  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: '1.25rem' }}>
      {chartData.length > 0 && (
        <div className="card" style={{ padding: '1.25rem' }}>
          <p style={{ fontWeight: 700, marginBottom: '1rem' }}>Low Stock — On Hand vs Reorder Level</p>
          <ResponsiveContainer width="100%" height={220}>
            <BarChart data={chartData} margin={{ left: -20 }}>
              <CartesianGrid strokeDasharray="3 3" vertical={false} />
              <XAxis dataKey="name" tick={{ fontSize: 10 }} />
              <YAxis tick={{ fontSize: 11 }} />
              <Tooltip />
              <Legend />
              <Bar dataKey="onHand"       name="On Hand"      fill="#ef4444" radius={[4,4,0,0]} />
              <Bar dataKey="reorderLevel" name="Reorder Level" fill="#6366f1" radius={[4,4,0,0]} />
            </BarChart>
          </ResponsiveContainer>
        </div>
      )}
      <ReportTable>
        <thead><tr><th>Product</th><th>Warehouse</th><th style={{ textAlign: 'right' }}>On Hand</th><th style={{ textAlign: 'right' }}>Reorder Level</th><th style={{ textAlign: 'right' }}>Deficit</th></tr></thead>
        <tbody>
          {data.length === 0 && <EmptyState icon="✅" text="No low stock items — all levels are healthy" />}
          {data.map((r, i) => (
            <tr key={i}>
              <td style={{ fontWeight: 600, color: 'hsl(var(--foreground))' }}>{r.productName}</td>
              <td>{r.warehouseName}</td>
              <td style={{ textAlign: 'right' }}><span className="badge badge-danger">{r.quantityOnHand}</span></td>
              <td style={{ textAlign: 'right' }} className="mono">{r.reorderLevel}</td>
              <td style={{ textAlign: 'right', color: 'hsl(var(--destructive))', fontFamily: "'JetBrains Mono', monospace" }}>
                -{((r.reorderLevel ?? 0) - (r.quantityOnHand ?? 0)).toFixed(0)}
              </td>
            </tr>
          ))}
        </tbody>
      </ReportTable>
    </div>
  )
}

/* ── OVERSTOCK ── */
function OverstockView({ data }) {
  return (
    <ReportTable>
      <thead><tr><th>Product</th><th>Warehouse</th><th style={{ textAlign: 'right' }}>On Hand</th><th style={{ textAlign: 'right' }}>Threshold</th><th style={{ textAlign: 'right' }}>Excess</th></tr></thead>
      <tbody>
        {data.length === 0 && <EmptyState icon="📦" text="No overstock detected" />}
        {data.map((r, i) => (
          <tr key={i}>
            <td style={{ fontWeight: 600, color: 'hsl(var(--foreground))' }}>{r.productName}</td>
            <td>{r.warehouseName}</td>
            <td style={{ textAlign: 'right', color: 'hsl(38 96% 55%)' }} className="mono">{r.quantityOnHand}</td>
            <td style={{ textAlign: 'right' }} className="mono">{r.overstockThreshold}</td>
            <td style={{ textAlign: 'right' }}><span className="badge badge-warning">+{Number(r.excessQuantity ?? 0).toFixed(0)}</span></td>
          </tr>
        ))}
      </tbody>
    </ReportTable>
  )
}

/* ── ABC ANALYSIS ── */
function AbcView({ data }) {
  if (!data) return (
    <div className="card" style={{ padding: '2rem', textAlign: 'center', opacity: 0.5 }}>No ABC data available</div>
  )

  const classBadge = { A: 'badge-success', B: 'badge-default', C: 'badge-secondary' }
  const classA = safeArr(data.classA)
  const classB = safeArr(data.classB)
  const classC = safeArr(data.classC)
  const allItems = [...classA, ...classB, ...classC]

  const pieData = [
    { name: 'Class A', value: classA.length },
    { name: 'Class B', value: classB.length },
    { name: 'Class C', value: classC.length },
  ].filter(d => d.value > 0)

  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: '1.25rem' }}>
      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(160px, 1fr))', gap: '1rem', alignItems: 'center' }}>
        {[{ cls: 'A', items: classA, label: 'High Value' }, { cls: 'B', items: classB, label: 'Medium Value' }, { cls: 'C', items: classC, label: 'Low Value' }].map(({ cls, items, label }) => (
          <div key={cls} className="card" style={{ padding: '1.25rem' }}>
            <div style={{ display: 'flex', alignItems: 'center', gap: '0.5rem', marginBottom: '0.5rem' }}>
              <span className={`badge ${classBadge[cls]}`}>Class {cls}</span>
            </div>
            <p style={{ fontSize: '1.75rem', fontWeight: 800, letterSpacing: '-0.04em' }}>{items.length}</p>
            <p style={{ fontSize: '0.75rem', color: 'hsl(var(--muted-foreground))' }}>{label} products</p>
          </div>
        ))}
        {pieData.length > 0 && (
          <div className="card" style={{ padding: '1.25rem', display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
            <PieChart width={120} height={120}>
              <Pie data={pieData} cx={55} cy={55} innerRadius={30} outerRadius={50} paddingAngle={2} dataKey="value">
                {pieData.map((_, i) => <Cell key={i} fill={['#22c55e', '#6366f1', '#9ca3af'][i]} />)}
              </Pie>
              <Tooltip />
            </PieChart>
          </div>
        )}
      </div>
      <ReportTable>
        <thead><tr><th>Product</th><th>SKU</th><th style={{ textAlign: 'right' }}>Qty</th><th style={{ textAlign: 'right' }}>Total Value</th><th style={{ textAlign: 'right' }}>Cumulative %</th><th>Class</th></tr></thead>
        <tbody>
          {allItems.length === 0 && <EmptyState icon="📊" text="No ABC data available" />}
          {allItems.map((r, i) => (
            <tr key={i}>
              <td style={{ fontWeight: 600, color: 'hsl(var(--foreground))' }}>{r.productName}</td>
              <td className="mono" style={{ color: 'hsl(var(--primary))', fontSize: '0.8125rem' }}>{r.sku}</td>
              <td style={{ textAlign: 'right' }} className="mono">{r.quantityOnHand}</td>
              <td style={{ textAlign: 'right' }} className="mono">₹{Number(r.totalValue ?? 0).toLocaleString()}</td>
              <td style={{ textAlign: 'right' }} className="mono">{r.cumulativeValuePercent}%</td>
              <td><span className={`badge ${classBadge[r.abcClass] || 'badge-secondary'}`}>{r.abcClass}</span></td>
            </tr>
          ))}
        </tbody>
      </ReportTable>
    </div>
  )
}

/* ── STOCK AGING ── */
function AgingView({ data }) {
  if (!data) return (
    <div className="card" style={{ padding: '2rem', textAlign: 'center', opacity: 0.5 }}>No aging data available</div>
  )

  const buckets = safeArr(data.buckets ?? data)
  if (buckets.length === 0) return (
    <div className="card" style={{ padding: '2rem', textAlign: 'center', opacity: 0.5 }}>No aging data available</div>
  )

  const colors = { '0–30 Days': 'badge-success', '31–60 Days': 'badge-default', '61–90 Days': 'badge-warning', '90+ Days': 'badge-danger' }

  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: '1rem' }}>
      {buckets.map((bucket, bi) => (
        <div key={bucket.bucketLabel ?? bi} className="card" style={{ overflow: 'hidden' }}>
          <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', padding: '0.875rem 1.25rem', borderBottom: '1px solid hsl(var(--border))' }}>
            <span className={`badge ${colors[bucket.bucketLabel] || 'badge-secondary'}`}>{bucket.bucketLabel}</span>
            <span style={{ fontSize: '0.75rem', color: 'hsl(var(--muted-foreground))' }}>
              {safeArr(bucket.products).length} items · {(bucket.totalQuantity ?? 0).toLocaleString()} units
            </span>
          </div>
          {safeArr(bucket.products).length > 0 && (
            <div className="data-table-wrap">
              <table className="data-table">
                <thead><tr><th>Product</th><th>Warehouse</th><th style={{ textAlign: 'right' }}>Qty</th><th style={{ textAlign: 'right' }}>Days Since Movement</th></tr></thead>
                <tbody>
                  {bucket.products.map((r, i) => (
                    <tr key={i}>
                      <td style={{ fontWeight: 600, color: 'hsl(var(--foreground))' }}>{r.productName}</td>
                      <td>{r.warehouseName}</td>
                      <td style={{ textAlign: 'right' }} className="mono">{r.quantity}</td>
                      <td style={{ textAlign: 'right' }} className="mono">{r.daysSinceLastMovement}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </div>
      ))}
    </div>
  )
}

/* ── TURNOVER ── */
function TurnoverView({ data }) {
  const badgeMap = { 'Fast Moving': 'badge-success', 'Normal': 'badge-default', 'Slow Moving': 'badge-warning', 'Dead Stock': 'badge-danger' }
  const chartData = data.slice(0, 10).map(r => ({
    name: (r.productName || '').slice(0, 10),
    ratio: r.turnoverRatio ?? 0,
  }))

  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: '1.25rem' }}>
      {chartData.length > 0 && (
        <div className="card" style={{ padding: '1.25rem' }}>
          <p style={{ fontWeight: 700, marginBottom: '1rem' }}>Turnover Ratios (top 10)</p>
          <ResponsiveContainer width="100%" height={200}>
            <BarChart data={chartData} margin={{ left: -20 }}>
              <CartesianGrid strokeDasharray="3 3" vertical={false} />
              <XAxis dataKey="name" tick={{ fontSize: 10 }} />
              <YAxis tick={{ fontSize: 11 }} />
              <Tooltip />
              <Bar dataKey="ratio" name="Turnover Ratio" radius={[4,4,0,0]} minPointSize={6}>
                {chartData.map((_, i) => <Cell key={i} fill={PALETTE[i % PALETTE.length]} />)}
              </Bar>
            </BarChart>
          </ResponsiveContainer>
        </div>
      )}
      <ReportTable>
        <thead><tr><th>Product</th><th>SKU</th><th style={{ textAlign: 'right' }}>Units Sold</th><th style={{ textAlign: 'right' }}>Avg On Hand</th><th style={{ textAlign: 'right' }}>Turnover</th><th style={{ textAlign: 'right' }}>Days in Inv.</th><th>Classification</th></tr></thead>
        <tbody>
          {data.length === 0 && <EmptyState icon="📈" text="No turnover data for this period" />}
          {data.map((r, i) => (
            <tr key={i}>
              <td style={{ fontWeight: 600, color: 'hsl(var(--foreground))' }}>{r.productName}</td>
              <td className="mono" style={{ color: 'hsl(var(--primary))', fontSize: '0.8125rem' }}>{r.sku}</td>
              <td style={{ textAlign: 'right' }} className="mono">{r.totalUnitsSold}</td>
              <td style={{ textAlign: 'right' }} className="mono">{Number(r.averageQtyOnHand ?? 0).toFixed(1)}</td>
              <td style={{ textAlign: 'right', fontWeight: 700, fontFamily: "'JetBrains Mono', monospace" }}>{r.turnoverRatio}x</td>
              <td style={{ textAlign: 'right' }} className="mono">{r.daysInInventory}</td>
              <td><span className={`badge ${badgeMap[r.classification] || 'badge-secondary'}`}>{r.classification}</span></td>
            </tr>
          ))}
        </tbody>
      </ReportTable>
    </div>
  )
}

/* ── TRANSACTIONS ── */
function TransactionsView({ data }) {
  const typeBadge = {
    PURCHASE: 'badge-success', SALE: 'badge-danger',
    TRANSFER_IN: 'badge-default', TRANSFER_OUT: 'badge-warning',
    ADJUSTMENT: 'badge-secondary', RETURN: 'badge-default',
  }
  return (
    <ReportTable>
      <thead><tr><th>Date</th><th>Product</th><th>Warehouse</th><th>Type</th><th style={{ textAlign: 'right' }}>Quantity</th><th>Reference</th></tr></thead>
      <tbody>
        {data.length === 0 && <EmptyState icon="🔄" text="No transactions found" />}
        {data.map((r, i) => (
          <tr key={i}>
            <td className="mono" style={{ fontSize: '0.75rem' }}>{r.transactionDate?.slice(0, 10)}</td>
            <td style={{ fontWeight: 600, color: 'hsl(var(--foreground))' }}>{r.productName}</td>
            <td>{r.warehouseName}</td>
            <td><span className={`badge ${typeBadge[r.transactionType] || 'badge-secondary'}`}>{r.transactionType}</span></td>
            <td style={{ textAlign: 'right' }} className="mono">{r.quantity}</td>
            <td style={{ fontSize: '0.75rem', opacity: 0.6 }}>{r.reference || '—'}</td>
          </tr>
        ))}
      </tbody>
    </ReportTable>
  )
}
