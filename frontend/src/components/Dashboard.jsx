import { useEffect, useState } from 'react'
import { api } from '../api'
import TankGauge from './TankGauge'

const COLOR_BY_PRODUCT = {
  Premium: 'var(--premium)',
  Regular: 'var(--regular)',
  Diesel: 'var(--diesel)',
}

export default function Dashboard({ stationName, t }) {
  const [inventory, setInventory] = useState([])
  const [todaySummary, setTodaySummary] = useState(null)
  const [error, setError] = useState('')

  useEffect(() => {
    load()
  }, [])

  async function load() {
    try {
      const now = new Date()
      const start = new Date(now.getFullYear(), now.getMonth(), now.getDate())
      const [inv, summary] = await Promise.all([
        api.getInventory(),
        api.getSummary(start.toISOString(), now.toISOString()),
      ])
      setInventory(inv)
      setTodaySummary(summary)
    } catch (err) {
      setError(err.message?.includes('Failed to fetch')
        ? t('dashboard_connectionError')
        : t('dashboard_serverError', { msg: err.message }))
    }
  }

  return (
    <div>
      <h1 className="page-title">{stationName}</h1>
      <p className="page-subtitle">{t('dashboard_subtitle')}</p>

      {error && <div className="alert alert-error">{error}</div>}

      <div className="grid-3" style={{ marginBottom: 24 }}>
        {inventory.map((p) => (
          <div key={p.productId} className="card product-card" style={{ '--product-color': COLOR_BY_PRODUCT[p.name] }}>
            <div className="product-name">{p.name}</div>
            <div style={{ display: 'flex', alignItems: 'center', gap: 12, marginTop: 8 }}>
              <TankGauge percent={p.percentFull} color={COLOR_BY_PRODUCT[p.name] || 'var(--accent)'} />
              <div>
                <div className="mono-num" style={{ fontSize: 22, fontWeight: 700 }}>
                  {p.currentStock.toLocaleString(undefined, { maximumFractionDigits: 1 })}
                </div>
                <div style={{ fontSize: 12, color: 'var(--text-muted)' }}>{t('dashboard_galAvailable')}</div>
              </div>
            </div>
          </div>
        ))}
      </div>

      {todaySummary && (
        <div className="card">
          <h2 style={{ fontSize: 18, marginBottom: 14 }}>{t('dashboard_today')}</h2>
          <div className="grid-3">
            <Metric label={t('dashboard_sold')} value={todaySummary.totalSales} />
            <Metric label={t('dashboard_bought')} value={todaySummary.totalPurchases} />
            <Metric label={t('dashboard_difference')} value={todaySummary.grossProfit} />
          </div>
        </div>
      )}
    </div>
  )
}

function Metric({ label, value }) {
  return (
    <div>
      <div style={{ fontSize: 12, color: 'var(--text-muted)', textTransform: 'uppercase', letterSpacing: '0.04em' }}>{label}</div>
      <div className="mono-num" style={{ fontSize: 24, fontWeight: 700 }}>
        {value.toLocaleString(undefined, { maximumFractionDigits: 2 })}
      </div>
    </div>
  )
}
