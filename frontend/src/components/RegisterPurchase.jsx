import { useEffect, useState } from 'react'
import { api } from '../api'
import Invoice from './Invoice'

export default function RegisterPurchase({ session, autoPrintInvoice, stationName, t }) {
  const [products, setProducts] = useState([])
  const [productId, setProductId] = useState('')
  const [gallons, setGallons] = useState('')
  const [unitPrice, setUnitPrice] = useState('')
  const [supplier, setSupplier] = useState('')
  const [invoiceNumber, setInvoiceNumber] = useState('')
  const [message, setMessage] = useState(null)
  const [submitting, setSubmitting] = useState(false)
  const [invoice, setInvoice] = useState(null)

  useEffect(() => {
    api.getProducts().then((data) => {
      setProducts(data)
      if (data.length) setProductId(String(data[0].id))
    })
  }, [])

  useEffect(() => {
    if (invoice && autoPrintInvoice) {
      const id = setTimeout(() => window.print(), 150)
      return () => clearTimeout(id)
    }
  }, [invoice, autoPrintInvoice])

  const selectedProduct = products.find((p) => String(p.id) === productId)

  async function handleSubmit(e) {
    e.preventDefault()
    setMessage(null)
    setSubmitting(true)
    try {
      const purchase = await api.registerPurchase({
        productId: Number(productId),
        quantityGallons: Number(gallons),
        unitPurchasePrice: Number(unitPrice),
        supplier,
        invoiceNumber,
      })
      setMessage({ type: 'success', text: t('purchase_success') })
      setInvoice({
        title: t('invoice_purchase'),
        number: purchase.invoiceNumber || invoiceNumber,
        date: new Date(purchase.dateTime || Date.now()).toLocaleString(),
        employee: session?.name || '',
        description: supplier || selectedProduct?.name || '',
        descLabel: t('purchase_supplier'),
        qtyLabel: t('purchase_gallonsReceived'),
        quantity: purchase.quantityGallons,
        unitPrice: purchase.unitPurchasePrice,
        total: purchase.totalCost,
        paymentMethod: '',
      })
      setGallons('')
      setUnitPrice('')
      setInvoiceNumber('')
    } catch (err) {
      setMessage({ type: 'error', text: err.message || t('purchase_errorFallback') })
    } finally {
      setSubmitting(false)
    }
  }

  return (
    <div>
      <h1 className="page-title">{t('purchase_title')}</h1>
      <p className="page-subtitle">{t('purchase_subtitle')}</p>

      <div className="card" style={{ maxWidth: 420 }}>
        {message && <div className={`alert alert-${message.type}`}>{message.text}</div>}

        <form onSubmit={handleSubmit}>
          <div className="form-row">
            <label>{t('purchase_product')}</label>
            <select value={productId} onChange={(e) => setProductId(e.target.value)}>
              {products.map((p) => (
                <option key={p.id} value={p.id}>{p.name}</option>
              ))}
            </select>
          </div>

          <div className="form-row">
            <label>{t('purchase_gallonsReceived')}</label>
            <input type="number" step="0.01" min="0.01" value={gallons} onChange={(e) => setGallons(e.target.value)} required />
          </div>

          <div className="form-row">
            <label>{t('purchase_unitPrice')}</label>
            <input type="number" step="0.01" min="0" value={unitPrice} onChange={(e) => setUnitPrice(e.target.value)} required />
          </div>

          <div className="form-row">
            <label>{t('purchase_supplier')}</label>
            <input value={supplier} onChange={(e) => setSupplier(e.target.value)} placeholder={t('purchase_supplierPlaceholder')} required />
          </div>

          <div className="form-row">
            <label>{t('purchase_invoiceNumber')}</label>
            <input value={invoiceNumber} onChange={(e) => setInvoiceNumber(e.target.value)} required />
          </div>

          <button type="submit" className="btn btn-primary btn-block" disabled={submitting || !gallons || Number(gallons) <= 0}>
            {submitting ? t('purchase_submitting') : t('purchase_submit')}
          </button>
        </form>
      </div>

      {invoice && (
        <Invoice data={invoice} stationName={stationName} t={t} onClose={() => setInvoice(null)} />
      )}
    </div>
  )
}
