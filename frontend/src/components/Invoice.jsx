export default function Invoice({ data, stationName, t, onClose }) {
  const money = (v) => Number(v).toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 })

  return (
    <div className="invoice-overlay">
      <div className="invoice-print">
        <div className="invoice">
          <div className="invoice-head">
            <div className="invoice-station">{stationName}</div>
            <div className="invoice-title">{data.title}</div>
          </div>

          <div className="invoice-meta">
            <div>{t('invoice_number')}: <b>{data.number}</b></div>
            <div>{t('invoice_date')}: {data.date}</div>
            <div>{t('invoice_employee')}: {data.employee}</div>
          </div>

          <table className="invoice-table">
            <thead>
              <tr>
                <th>{data.descLabel}</th>
                <th className="ta-r">{data.qtyLabel}</th>
                <th className="ta-r">{t('invoice_unitPrice')}</th>
                <th className="ta-r">{t('invoice_amount')}</th>
              </tr>
            </thead>
            <tbody>
              <tr>
                <td>{data.description}</td>
                <td className="ta-r">{data.quantity}</td>
                <td className="ta-r">{money(data.unitPrice)}</td>
                <td className="ta-r">{money(data.total)}</td>
              </tr>
            </tbody>
          </table>

          <div className="invoice-total">
            <span>{t('invoice_total')}</span>
            <span>{money(data.total)}</span>
          </div>

          {data.paymentMethod && (
            <div className="invoice-footer">
              <span>{t('sale_paymentMethod')}:</span>
              <span><b>{data.paymentMethod}</b></span>
            </div>
          )}

          <div className="invoice-thanks">{t('invoice_thanks')}</div>
        </div>
      </div>

      <div className="invoice-actions">
        <button className="btn btn-primary" onClick={() => window.print()}>{t('invoice_print')}</button>
        <button className="btn btn-secondary" onClick={onClose}>{t('invoice_close')}</button>
      </div>
    </div>
  )
}