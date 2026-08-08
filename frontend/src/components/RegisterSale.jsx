import { useEffect, useState } from 'react'
import { api } from '../api'

export default function RegisterSale({ employeeId, t }) {
  const [products, setProducts] = useState([])
  const [productId, setProductId] = useState('')
  const [gallons, setGallons] = useState('')
  const [paymentMethod, setPaymentMethod] = useState('Cash')
  const [message, setMessage] = useState(null)
  const [submitting, setSubmitting] = useState(false)

  useEffect(() => {
    api.getProducts().then((data) => {
      setProducts(data)
      if (data.length) setProductId(String(data[0].id))
    })
  }, [])

  const selectedProduct = products.find((p) => String(p.id) === productId)
  const total = selectedProduct && gallons ? selectedProduct.currentSalePrice * Number(gallons) : 0

  async function handleSubmit(e) {
    e.preventDefault()
    setMessage(null)
    setSubmitting(true)
    try {
      const sale = await api.registerSale({
        productId: Number(productId),
        employeeId,
        quantityGallons: Number(gallons),
        paymentMethod,
      })
      setMessage({ type: 'success', text: t('sale_success', { ticket: sale.ticketNumber }) })
      setGallons('')
    } catch (err) {
      setMessage({ type: 'error', text: err.message || t('sale_errorFallback') })
    } finally {
      setSubmitting(false)
    }
  }

  return (
    <div>
      <h1 className="page-title">{t('sale_title')}</h1>
      <p className="page-subtitle">{t('sale_subtitle')}</p>

      <div className="card" style={{ maxWidth: 420 }}>
        {message && <div className={`alert alert-${message.type}`}>{message.text}</div>}

        <form onSubmit={handleSubmit}>
          <div className="form-row">
            <label>{t('sale_product')}</label>
            <select value={productId} onChange={(e) => setProductId(e.target.value)}>
              {products.map((p) => (
                <option key={p.id} value={p.id}>{p.name} — {p.currentSalePrice}/gal</option>
              ))}
            </select>
          </div>

          <div className="form-row">
            <label>{t('sale_gallons')}</label>
            <input
              type="number"
              step="0.01"
              min="0"
              value={gallons}
              onChange={(e) => setGallons(e.target.value)}
              placeholder="0.00"
              required
            />
          </div>

          <div className="form-row">
            <label>{t('sale_paymentMethod')}</label>
            <select value={paymentMethod} onChange={(e) => setPaymentMethod(e.target.value)}>
              <option value="Cash">{t('sale_cash')}</option>
              <option value="Card">{t('sale_card')}</option>
              <option value="Transfer">{t('sale_transfer')}</option>
            </select>
          </div>

          <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', margin: '20px 0' }}>
            <span style={{ color: 'var(--text-muted)', fontSize: 14 }}>{t('sale_totalDue')}</span>
            <span className="mono-num" style={{ fontSize: 26, fontWeight: 700 }}>
              {total.toLocaleString(undefined, { maximumFractionDigits: 2 })}
            </span>
          </div>

          <button type="submit" className="btn btn-primary btn-block" disabled={submitting || !gallons}>
            {submitting ? t('sale_submitting') : t('sale_submit')}
          </button>
        </form>
      </div>
    </div>
  )
}
