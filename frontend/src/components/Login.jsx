import { useState } from 'react'
import { api } from '../api'

export default function Login({ onLogin, t }) {
  const [username, setUsername] = useState('')
  const [password, setPassword] = useState('')
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(false)

  async function handleSubmit(e) {
    e.preventDefault()
    setError('')
    setLoading(true)
    try {
      const data = await api.login(username, password)
      onLogin(data)
    } catch (err) {
      setError(t('login_error'))
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="login-screen">
      <div className="card login-card">
        <div className="brand" style={{ marginBottom: 18 }}>
          <div className="brand-mark" />
          <span className="brand-title">{t('appName')}</span>
        </div>
        <h1 className="login-title">{t('login_title')}</h1>
        <p className="page-subtitle">{t('login_subtitle')}</p>

        {error && <div className="alert alert-error">{error}</div>}

        <form onSubmit={handleSubmit}>
          <div className="form-row">
            <label htmlFor="username">{t('login_username')}</label>
            <input id="username" value={username} onChange={(e) => setUsername(e.target.value)} placeholder="user1" autoFocus />
          </div>
          <div className="form-row">
            <label htmlFor="password">{t('login_password')}</label>
            <input id="password" type="password" value={password} onChange={(e) => setPassword(e.target.value)} placeholder="••••••" />
          </div>
          <button type="submit" className="btn btn-primary btn-block" disabled={loading}>
            {loading ? t('login_submitting') : t('login_submit')}
          </button>
        </form>
      </div>
    </div>
  )
}
