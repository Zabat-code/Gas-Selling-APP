import { useEffect, useState } from 'react'
import { api } from '../api'

export default function Options({ session, theme, onChangeTheme, language, onChangeLanguage, stationName, onChangeStationName, t }) {
  return (
    <div>
      <h1 className="page-title">{t('options_title')}</h1>
      <p className="page-subtitle">
        {t('options_subtitle')}{session.isAdmin ? t('options_subtitleUsers') : ''}
      </p>

      <div className="card" style={{ maxWidth: 460, marginBottom: 20 }}>
        <h2 style={{ fontSize: 18, marginBottom: 14 }}>{t('options_appearance')}</h2>
        <div style={{ display: 'flex', gap: 10, marginBottom: 16 }}>
          <button
            className={`nav-btn ${theme === 'dark' ? 'active' : ''}`}
            style={{ flex: 1, padding: '10px 0' }}
            onClick={() => onChangeTheme('dark')}
          >
            🌙 {t('options_night')}
          </button>
          <button
            className={`nav-btn ${theme === 'light' ? 'active' : ''}`}
            style={{ flex: 1, padding: '10px 0' }}
            onClick={() => onChangeTheme('light')}
          >
            ☀️ {t('options_day')}
          </button>
        </div>

        <label style={{ display: 'block', marginBottom: 8 }}>{t('options_language')}</label>
        <div style={{ display: 'flex', gap: 10 }}>
          <button
            className={`nav-btn ${language === 'en' ? 'active' : ''}`}
            style={{ flex: 1, padding: '10px 0' }}
            onClick={() => onChangeLanguage('en')}
          >
            English
          </button>
          <button
            className={`nav-btn ${language === 'es' ? 'active' : ''}`}
            style={{ flex: 1, padding: '10px 0' }}
            onClick={() => onChangeLanguage('es')}
          >
            Español
          </button>
        </div>
      </div>

      <StationNameCard stationName={stationName} onChangeStationName={onChangeStationName} t={t} />

      {session.isAdmin && <CreateUser session={session} t={t} />}
    </div>
  )
}

function StationNameCard({ stationName, onChangeStationName, t }) {
  const [value, setValue] = useState(stationName)
  const [message, setMessage] = useState(null)

  useEffect(() => setValue(stationName), [stationName])

  async function save(e) {
    e.preventDefault()
    setMessage(null)
    try {
      await api.updateSettings(value)
      onChangeStationName(value)
      setMessage({ type: 'success', text: t('options_stationNameUpdated') })
    } catch (err) {
      setMessage({ type: 'error', text: err.message || t('options_stationNameError') })
    }
  }

  return (
    <div className="card" style={{ maxWidth: 460, marginBottom: 20 }}>
      <h2 style={{ fontSize: 18, marginBottom: 14 }}>{t('options_stationName')}</h2>
      {message && <div className={`alert alert-${message.type}`}>{message.text}</div>}
      <form onSubmit={save}>
        <div className="form-row">
          <label>{t('options_stationNameHelp')}</label>
          <input value={value} onChange={(e) => setValue(e.target.value)} />
        </div>
        <button type="submit" className="btn btn-primary btn-block">{t('save')}</button>
      </form>
    </div>
  )
}

function CreateUser({ session, t }) {
  const [employees, setEmployees] = useState([])
  const [name, setName] = useState('')
  const [username, setUsername] = useState('')
  const [password, setPassword] = useState('')
  const [isAdmin, setIsAdmin] = useState(false)
  const [message, setMessage] = useState(null)
  const [submitting, setSubmitting] = useState(false)

  useEffect(() => { load() }, [])

  function load() {
    api.getEmployees().then(setEmployees).catch(() => {})
  }

  async function create(e) {
    e.preventDefault()
    setMessage(null)
    setSubmitting(true)
    try {
      await api.createEmployee({
        requesterId: session.employeeId,
        name,
        username,
        password,
        isAdmin,
      })
      setMessage({ type: 'success', text: t('options_userCreated', { username }) })
      setName('')
      setUsername('')
      setPassword('')
      setIsAdmin(false)
      load()
    } catch (err) {
      setMessage({ type: 'error', text: err.message || t('options_userCreateError') })
    } finally {
      setSubmitting(false)
    }
  }

  return (
    <div className="card" style={{ maxWidth: 460 }}>
      <h2 style={{ fontSize: 18, marginBottom: 14 }}>{t('options_users')}</h2>

      {employees.length > 0 && (
        <table style={{ marginBottom: 18 }}>
          <thead>
            <tr><th>{t('options_colName')}</th><th>{t('options_colUsername')}</th><th>{t('options_colAdmin')}</th></tr>
          </thead>
          <tbody>
            {employees.map((e) => (
              <tr key={e.id}>
                <td>{e.name}</td>
                <td>{e.username}</td>
                <td>{e.isAdmin ? t('yes') : t('no')}</td>
              </tr>
            ))}
          </tbody>
        </table>
      )}

      {message && <div className={`alert alert-${message.type}`}>{message.text}</div>}

      <form onSubmit={create}>
        <div className="form-row">
          <label>{t('options_userName')}</label>
          <input value={name} onChange={(e) => setName(e.target.value)} required />
        </div>
        <div className="form-row">
          <label>{t('options_userUsername')}</label>
          <input value={username} onChange={(e) => setUsername(e.target.value)} required />
        </div>
        <div className="form-row">
          <label>{t('options_userPassword')}</label>
          <input type="password" value={password} onChange={(e) => setPassword(e.target.value)} required />
        </div>
        <div className="form-row" style={{ flexDirection: 'row', alignItems: 'center', gap: 8 }}>
          <input type="checkbox" id="isAdmin" checked={isAdmin} onChange={(e) => setIsAdmin(e.target.checked)} style={{ width: 'auto' }} />
          <label htmlFor="isAdmin" style={{ margin: 0 }}>{t('options_userIsAdmin')}</label>
        </div>
        <button type="submit" className="btn btn-primary btn-block" disabled={submitting}>
          {submitting ? t('options_creating') : t('options_createUser')}
        </button>
      </form>
    </div>
  )
}
