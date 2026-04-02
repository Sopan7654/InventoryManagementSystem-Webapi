// src/components/Modal.jsx
import { useEffect } from 'react'
import { X } from 'lucide-react'

export default function Modal({ title, children, onClose }) {
  useEffect(() => {
    const h = (e) => { if (e.key === 'Escape') onClose() }
    window.addEventListener('keydown', h)
    return () => window.removeEventListener('keydown', h)
  }, [onClose])

  return (
    <div
      className="dialog-overlay"
      onClick={(e) => e.target === e.currentTarget && onClose()}
    >
      <div className="dialog-panel">
        <div className="dialog-header">
          <h2 className="dialog-title">{title}</h2>
          <button className="dialog-close" onClick={onClose} aria-label="Close">
            <X size={14} strokeWidth={2.5} />
          </button>
        </div>
        <div className="dialog-body">{children}</div>
      </div>
    </div>
  )
}
