import { useState } from 'react'
import { api } from '../api'

function startOfTodayISO() {
  const d = new Date()
  return new Date(d.getFullYear(), d.getMonth(), d.getDate()).toISOString().slice(0, 10)
}

const COLOR_BY_PRODUCT = {
  Premium: 'var(--premium)',
  Regular: 'var(--regular)',
  Diesel: 'var(--diesel)',
}

export default function Reports({ t }) {
  const [from, setFrom] = useState(startOfTodayISO())
  const [to, setTo] = useState(startOfTodayISO())
  const [summary, setSummary] = useState(null)
  const [error, setError] = useState('')

  async function search(e) {
    e?.preventDefault()
    setError('')
    try {
      const data = await api.getSummary(new Date(from).toISOString(), new Date(to + 'T23:59:59').toISOString())
      setSummary(data)
    } catch (err) {
      setError(t('reports_errorFallback', { msg: err.message }))
    }
  }

  return (
    <div>
      <h1 className="page-title">{t('reports_title')}</h1>
      <p className="page-subtitle">{t('reports_subtitle')}</p>

      <div className="card" style={{ marginBottom: 20 }}>
        <form onSubmit={search} style={{ display: 'flex', gap: 14, alignItems: 'flex-end', flexWrap: 'wrap' }}>
          <div className="form-row" style={{ marginBottom: 0 }}>
            <label>{t('reports_from')}</label>
            <input type="date" value={from} onChange={(e) => setFrom(e.target.value)} />
          </div>
          <div className="form-row" style={{ marginBottom: 0 }}>
            <label>{t('reports_to')}</label>
            <input type="date" value={to} onChange={(e) => setTo(e.target.value)} />
          </div>
          <button className="btn btn-primary" type="submit">{t('reports_generate')}</button>
        </form>
      </div>

      {error && <div className="alert alert-error">{error}</div>}

      {summary && (
        <>
          <div className="grid-3" style={{ marginBottom: 20 }}>
            <div className="card">
              <div style={{ fontSize: 12, color: 'var(--text-muted)' }}>{t('reports_totalSold').toUpperCase()}</div>
              <div className="mono-num" style={{ fontSize: 24, fontWeight: 700 }}>{summary.totalSales.toLocaleString(undefined, { maximumFractionDigits: 2 })}</div>
            </div>
            <div className="card">
              <div style={{ fontSize: 12, color: 'var(--text-muted)' }}>{t('reports_totalBought').toUpperCase()}</div>
              <div className="mono-num" style={{ fontSize: 24, fontWeight: 700 }}>{summary.totalPurchases.toLocaleString(undefined, { maximumFractionDigits: 2 })}</div>
            </div>
            <div className="card">
              <div style={{ fontSize: 12, color: 'var(--text-muted)' }}>{t('reports_difference').toUpperCase()}</div>
              <div className="mono-num" style={{ fontSize: 24, fontWeight: 700 }}>{summary.grossProfit.toLocaleString(undefined, { maximumFractionDigits: 2 })}</div>
            </div>
          </div>

          <div className="card">
            <table>
              <thead>
                <tr>
                  <th>{t('reports_product')}</th>
                  <th>{t('reports_galSold')}</th>
                  <th>{t('reports_totalSoldCol')}</th>
                  <th>{t('reports_galBought')}</th>
                  <th>{t('reports_totalBoughtCol')}</th>
                </tr>
              </thead>
              <tbody>
                {summary.byProduct.map((p) => (
                  <tr key={p.product}>
                    <td>
                      <span className="pill" style={{ background: 'var(--surface-raised)', color: COLOR_BY_PRODUCT[p.product] }}>{p.product}</span>
                    </td>
                    <td className="mono-num">{p.gallonsSold.toLocaleString(undefined, { maximumFractionDigits: 1 })}</td>
                    <td className="mono-num">{p.totalSold.toLocaleString(undefined, { maximumFractionDigits: 2 })}</td>
                    <td className="mono-num">{p.gallonsPurchased.toLocaleString(undefined, { maximumFractionDigits: 1 })}</td>
                    <td className="mono-num">{p.totalPurchased.toLocaleString(undefined, { maximumFractionDigits: 2 })}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </>
      )}
    </div>
  )
}
