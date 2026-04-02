// src/components/Toast.jsx
import { useEffect } from 'react'
import { CheckCircle2, XCircle, Info, X } from 'lucide-react'

const variants = {
  success: { cls: 'toast-success', Icon: CheckCircle2, iconColor: 'hsl(142 71% 45%)' },
  error:   { cls: 'toast-error',   Icon: XCircle,      iconColor: 'hsl(var(--destructive))' },
  info:    { cls: 'toast-info',    Icon: Info,          iconColor: 'hsl(var(--primary))' },
}

export default function Toast({ message, type = 'success', onClose }) {
  useEffect(() => {
    const t = setTimeout(onClose, 4000)
    return () => clearTimeout(t)
  }, [onClose])

  const { cls, Icon, iconColor } = variants[type] || variants.success

  return (
    <div className={`toast ${cls}`} role="alert" style={{ position: 'relative', overflow: 'hidden' }}>
      <div className="toast-icon-wrap">
        <Icon size={16} color={iconColor} strokeWidth={2} />
      </div>
      <span style={{ flex: 1, fontSize: '0.8125rem', fontWeight: 500, color: 'hsl(var(--foreground))' }}>
        {message}
      </span>
      <button
        onClick={onClose}
        style={{ background: 'none', border: 'none', cursor: 'pointer', color: 'hsl(var(--muted-foreground))', padding: '2px', display: 'flex', transition: 'color 0.15s' }}
        onMouseEnter={e => e.target.style.color = 'hsl(var(--foreground))'}
        onMouseLeave={e => e.target.style.color = 'hsl(var(--muted-foreground))'}
        aria-label="Dismiss"
      >
        <X size={14} />
      </button>
      <div className="toast-bar" />
    </div>
  )
}
