// src/pages/Suppliers.jsx
import { useState, useEffect } from 'react'
import { Plus, Search, Globe, Mail, Phone } from 'lucide-react'
import { getSuppliers, createSupplier } from '../api/inventoryApi'
import PageHeader from '../components/PageHeader'
import Modal from '../components/Modal'
import Toast from '../components/Toast'
import LoadingSpinner from '../components/LoadingSpinner'

const EMPTY = { supplierName: '', email: '', phone: '', website: '', address: '' }

export default function Suppliers() {
  const [records, setRecords] = useState([])
  const [loading, setLoading] = useState(true)
  const [modal, setModal]     = useState(false)
  const [form, setForm]       = useState(EMPTY)
  const [toast, setToast]     = useState(null)
  const [saving, setSaving]   = useState(false)
  const [search, setSearch]   = useState('')

  const reload = () => getSuppliers().then(r => setRecords(r.data || [])).finally(() => setLoading(false))
  useEffect(() => { reload() }, [])

  const handleSave = async () => {
    setSaving(true)
    try {
      await createSupplier(form)
      setToast({ message: 'Supplier created!', type: 'success' })
      setModal(false); setForm(EMPTY); reload()
    } catch (e) { setToast({ message: e.message, type: 'error' }) }
    finally { setSaving(false) }
  }

  const filtered = records.filter(s => s.supplierName?.toLowerCase().includes(search.toLowerCase()))
  if (loading) return <LoadingSpinner />

  return (
    <div className="anim-fade-in">
      <PageHeader
        title="Suppliers"
        subtitle={`${records.length} suppliers`}
        actions={
          <button className="btn btn-default" onClick={() => setModal(true)}>
            <Plus size={15} strokeWidth={2.5} /> Add Supplier
          </button>
        }
      />

      <div style={{ marginBottom: '1rem' }}>
        <div className="search-input-wrap" style={{ maxWidth: 300 }}>
          <Search size={15} className="search-icon-inner" />
          <input className="input" placeholder="Search suppliers…" value={search} onChange={e => setSearch(e.target.value)} />
        </div>
      </div>

      <div className="card" style={{ overflow: 'hidden' }}>
        <div className="data-table-wrap">
          <table className="data-table">
            <thead>
              <tr>
                <th>Supplier Name</th>
                <th>Email</th>
                <th>Phone</th>
                <th>Website</th>
              </tr>
            </thead>
            <tbody>
              {filtered.length === 0 && (
                <tr><td colSpan={4}><div className="empty-state"><span className="empty-icon">🤝</span><p className="empty-title">No suppliers found</p></div></td></tr>
              )}
              {filtered.map(s => (
                <tr key={s.supplierId}>
                  <td style={{ fontWeight: 600, color: 'hsl(var(--foreground))' }}>{s.supplierName}</td>
                  <td>
                    {s.email
                      ? <span style={{ display: 'flex', alignItems: 'center', gap: '0.375rem' }}><Mail size={13} />{s.email}</span>
                      : <span style={{ opacity: 0.4 }}>—</span>
                    }
                  </td>
                  <td>
                    {s.phone
                      ? <span style={{ display: 'flex', alignItems: 'center', gap: '0.375rem' }}><Phone size={13} />{s.phone}</span>
                      : <span style={{ opacity: 0.4 }}>—</span>
                    }
                  </td>
                  <td>
                    {s.website
                      ? <span style={{ display: 'flex', alignItems: 'center', gap: '0.375rem', color: 'hsl(var(--primary))' }}><Globe size={13} />{s.website}</span>
                      : <span style={{ opacity: 0.4 }}>—</span>
                    }
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>

      {modal && (
        <Modal title="New Supplier" onClose={() => setModal(false)}>
          <div style={{ display: 'flex', flexDirection: 'column', gap: '1rem' }}>
            {[['Name *', 'supplierName', 'text', 'TechSource India'], ['Email', 'email', 'email', 'orders@supplier.com'], ['Phone', 'phone', 'text', '+91-22-1234-5678'], ['Website', 'website', 'text', 'www.supplier.com'], ['Address', 'address', 'text', 'Mumbai, India']].map(([label, key, type, ph]) => (
              <div key={key}>
                <label className="label">{label}</label>
                <input type={type} className="input" placeholder={ph} value={form[key]} onChange={e => setForm(f => ({ ...f, [key]: e.target.value }))} />
              </div>
            ))}
            <div style={{ display: 'flex', gap: '0.75rem' }}>
              <button className="btn btn-default" style={{ flex: 1 }} onClick={handleSave} disabled={saving}>
                {saving ? 'Saving…' : 'Create Supplier'}
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
