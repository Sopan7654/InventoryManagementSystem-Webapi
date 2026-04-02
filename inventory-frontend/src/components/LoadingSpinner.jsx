// src/components/LoadingSpinner.jsx
export default function LoadingSpinner({ text = 'Loading...' }) {
  return (
    <div style={{ display: 'flex', flexDirection: 'column', alignItems: 'center', justifyContent: 'center', padding: '5rem 2rem', gap: '1rem' }}>
      <div className="spinner" role="status" aria-label={text} />
      <p style={{ fontSize: '0.8125rem', color: 'hsl(var(--muted-foreground))', fontWeight: 500 }}>{text}</p>
    </div>
  )
}
