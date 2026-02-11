import { Link } from 'react-router-dom'

interface GameFrameProps {
  title: string
  children: React.ReactNode
  metrics?: React.ReactNode
}

export default function GameFrame({ title, children, metrics }: GameFrameProps) {
  return (
    <div className="game-container">
      <div style={{ display: 'flex', alignItems: 'center', gap: '1rem', marginBottom: '1rem', flexWrap: 'wrap' }}>
        <Link to="/" className="secondary" style={{ padding: '0.4rem 0.8rem' }}>← Inicio</Link>
        <h1 style={{ margin: 0 }}>{title}</h1>
      </div>
      {metrics && <div className="metrics">{metrics}</div>}
      {children}
    </div>
  )
}
