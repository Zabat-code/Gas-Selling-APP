import { useEffect, useState } from 'react'
import { api } from '../api'

export default function Prices({ session, t }) {
  const isAdmin = session?.isAdmin
  const canModifyPrices = !!(session?.isAdmin || session?.canModifyPrices)
  const [products, setProducts] = useState([])
  const [message, setMessage] = useState(null)

  useEffect(() => { load() }, [])

  function load() {
    api.getProducts().then(setProducts)
  }

  async function savePrices(product, currentSalePrice, currentPurchasePrice) {
    setMessage(null)
    try {
            await api.updatePrices(product.id, Number(currentSalePrice), Number(currentPurchasePrice))
      setMessage({ type: 'success', text: t('prices_pricesUpdated', { name: product.name }) })
      load()
    } catch (err) {
      setMessage({ type: 'error', text: err.message || t('prices_errorFallback') })
    }
  }

  async function saveCapacity(product, tankCapacityGallons) {
    setMessage(null)
    try {
            await api.updateTankCapacity(product.id, Number(tankCapacityGallons))
      setMessage({ type: 'success', text: t('prices_capacityUpdated', { name: product.name }) })
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
          <PriceCard
            key={p.id}
            product={p}
            isAdmin={isAdmin}
            canModifyPrices={canModifyPrices}
            onSavePrices={savePrices}
            onSaveCapacity={saveCapacity}
            t={t}
          />
        ))}
      </div>
    </div>
  )
}

function PriceCard({ product, isAdmin, canModifyPrices, onSavePrices, onSaveCapacity, t }) {
  const [salePrice, setSalePrice] = useState(product.currentSalePrice)
  const [purchasePrice, setPurchasePrice] = useState(product.currentPurchasePrice)
  const [capacity, setCapacity] = useState(product.tankCapacityGallons)

  return (
    <div className="card">
      <div className="product-name" style={{ marginBottom: 10 }}>{product.name}</div>
      <div className="form-row">
        <label>{t('prices_salePrice')}</label>
        <input
          type="number" step="0.01" min="0"
          value={salePrice}
          readOnly={!canModifyPrices}
          disabled={!canModifyPrices}
          onChange={(e) => setSalePrice(e.target.value)}
          title={canModifyPrices ? '' : t('prices_pricesPermissionHelp')}
        />
      </div>
      <div className="form-row">
        <label>{t('prices_purchasePrice')}</label>
        <input
          type="number" step="0.01" min="0"
          value={purchasePrice}
          readOnly={!canModifyPrices}
          disabled={!canModifyPrices}
          onChange={(e) => setPurchasePrice(e.target.value)}
        />
      </div>
      <button className="btn btn-primary btn-block" disabled={!canModifyPrices} onClick={() => onSavePrices(product, salePrice, purchasePrice)}>
        {t('prices_savePrices')}
      </button>

      <div style={{ margin: '16px 0 8px', paddingTop: 12, borderTop: '1px solid var(--border, #333)' }} />
      <div className="form-row">
        <label>{t('prices_capacity')}</label>
        <input
          type="number"
          step="1"
          min="1"
          value={capacity}
          readOnly={!isAdmin}
          disabled={!isAdmin}
          onChange={(e) => setCapacity(e.target.value)}
          title={isAdmin ? '' : t('prices_capacityAdminOnlyHelp')}
        />
      </div>
      <button
        className="btn btn-secondary btn-block"
        disabled={!isAdmin}
        onClick={() => onSaveCapacity(product, capacity)}
      >
        {t('prices_saveCapacity')}
      </button>
    </div>
  )
}
