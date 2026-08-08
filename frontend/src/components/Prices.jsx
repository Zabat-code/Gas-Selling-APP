import { useEffect, useState } from 'react'
import { api } from '../api'

export default function Prices({ t }) {
  const [products, setProducts] = useState([])
  const [message, setMessage] = useState(null)

  useEffect(() => { load() }, [])

  function load() {
    api.getProducts().then(setProducts)
  }

  async function save(product, currentSalePrice, currentPurchasePrice, tankCapacityGallons) {
    setMessage(null)
    try {
      await api.updateProduct(product.id, Number(currentSalePrice), Number(currentPurchasePrice), Number(tankCapacityGallons))
      setMessage({ type: 'success', text: t('prices_updated', { name: product.name }) })
      load()
    } catch (err) {
      setMessage({ type: 'error', text: err.message || t('prices_errorFallback') })
    }
  }

  return (
    <div>
      <h1 className="page-title">{t('prices_title')}</h1>
      <p className="page-subtitle">{t('prices_subtitle')}</p>

      {message && <div className={`alert alert-${message.type}`}>{message.text}</div>}

      <div className="grid-3">
        {products.map((p) => (
          <PriceCard key={p.id} product={p} onSave={save} t={t} />
        ))}
      </div>
    </div>
  )
}

function PriceCard({ product, onSave, t }) {
  const [salePrice, setSalePrice] = useState(product.currentSalePrice)
  const [purchasePrice, setPurchasePrice] = useState(product.currentPurchasePrice)
  const [capacity, setCapacity] = useState(product.tankCapacityGallons)

  return (
    <div className="card">
      <div className="product-name" style={{ marginBottom: 10 }}>{product.name}</div>
      <div className="form-row">
        <label>{t('prices_salePrice')}</label>
        <input type="number" step="0.01" value={salePrice} onChange={(e) => setSalePrice(e.target.value)} />
      </div>
      <div className="form-row">
        <label>{t('prices_purchasePrice')}</label>
        <input type="number" step="0.01" value={purchasePrice} onChange={(e) => setPurchasePrice(e.target.value)} />
      </div>
      <div className="form-row">
        <label>{t('prices_capacity')}</label>
        <input type="number" step="1" min="1" value={capacity} onChange={(e) => setCapacity(e.target.value)} />
      </div>
      <button className="btn btn-primary btn-block" onClick={() => onSave(product, salePrice, purchasePrice, capacity)}>
        {t('save')}
      </button>
    </div>
  )
}
