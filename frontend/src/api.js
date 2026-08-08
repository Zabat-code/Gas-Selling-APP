const BASE_URL = 'http://localhost:5000/api'

async function request(path, options = {}) {
  const res = await fetch(`${BASE_URL}${path}`, {
    headers: { 'Content-Type': 'application/json' },
    ...options,
  })

  if (!res.ok) {
    const text = await res.text().catch(() => '')
    throw new Error(text || `Error ${res.status}`)
  }

  const contentType = res.headers.get('content-type') || ''
  return contentType.includes('application/json') ? res.json() : null
}

export const api = {
  login: (username, password) =>
    request('/auth/login', { method: 'POST', body: JSON.stringify({ username, password }) }),

  getProducts: () => request('/products'),
  updateProduct: (id, currentSalePrice, currentPurchasePrice, tankCapacityGallons) =>
    request(`/products/${id}`, {
      method: 'PUT',
      body: JSON.stringify({ currentSalePrice, currentPurchasePrice, tankCapacityGallons }),
    }),

  getInventory: () => request('/inventory'),

  registerPurchase: (data) => request('/purchases', { method: 'POST', body: JSON.stringify(data) }),
  getPurchases: (params = '') => request(`/purchases${params}`),

  registerSale: (data) => request('/sales', { method: 'POST', body: JSON.stringify(data) }),
  getSales: (params = '') => request(`/sales${params}`),

  getSummary: (from, to) =>
    request(`/reports/summary?from=${encodeURIComponent(from)}&to=${encodeURIComponent(to)}`),

  getEmployees: () => request('/employees'),
  createEmployee: (data) => request('/employees', { method: 'POST', body: JSON.stringify(data) }),

  getSettings: () => request('/settings'),
  updateSettings: (stationName) =>
    request('/settings', { method: 'PUT', body: JSON.stringify({ stationName }) }),
}
