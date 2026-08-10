const BASE_URL = 'http://localhost:5000/api'
const TOKEN_KEY = 'auth_token'

// Inject the bearer JWT (saved by `login`) into every request. The backend
// identifies the caller from the token, never from the body.
function authHeaders() {
  const token = localStorage.getItem(TOKEN_KEY)
  return token ? { Authorization: `Bearer ${token}` } : {}
}

async function request(path, options = {}) {
  const res = await fetch(`${BASE_URL}${path}`, {
    headers: { 'Content-Type': 'application/json', ...authHeaders() },
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
  login: async (username, password) => {
    const data = await request('/auth/login', {
      method: 'POST',
      body: JSON.stringify({ username, password }),
    })
    localStorage.setItem(TOKEN_KEY, data.token)
    return data
  },
  logout: () => localStorage.removeItem(TOKEN_KEY),

  getProducts: () => request('/products'),
  updatePrices: (id, currentSalePrice, currentPurchasePrice) =>
    request(`/products/${id}`, {
      method: 'PUT',
      body: JSON.stringify({ currentSalePrice, currentPurchasePrice }),
    }),
  updateTankCapacity: (id, tankCapacityGallons) =>
    request(`/products/${id}/tank-capacity`, {
      method: 'PUT',
      body: JSON.stringify({ tankCapacityGallons }),
    }),

  getInventory: () => request('/inventory'),

  registerPurchase: (data) => request('/purchases', { method: 'POST', body: JSON.stringify(data) }),
  getPurchases: (params = '') => request(`/purchases${params}`),

  registerSale: (data) => request('/sales', { method: 'POST', body: JSON.stringify(data) }),
  getSales: (params = '') => request(`/sales${params}`),
  getSale: (id) => request(`/sales/${id}`),

  getSummary: (from, to) =>
    request(`/reports/summary?from=${encodeURIComponent(from)}&to=${encodeURIComponent(to)}`),

  getEmployees: () => request('/employees'),
  createEmployee: (data) => request('/employees', { method: 'POST', body: JSON.stringify(data) }),
  updateEmployeePermissions: (employeeId, canModifyPrices) =>
    request(`/employees/${employeeId}/permissions`, {
      method: 'PUT',
      body: JSON.stringify({ canModifyPrices }),
    }),

  getSettings: () => request('/settings'),
  updateSettings: (stationName, autoPrintInvoice, taxRate) =>
    request('/settings', { method: 'PUT', body: JSON.stringify({ stationName, autoPrintInvoice, taxRate }) }),
}
