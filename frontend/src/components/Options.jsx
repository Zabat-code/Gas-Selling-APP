import { useEffect, useState } from 'react'
import { api } from '../api'

export default function Options({ session, theme, onChangeTheme, language, onChangeLanguage, stationName, onChangeStationName, autoPrintInvoice, onChangeAutoPrint, t }) {
  return (
    <div>
      <h1 className="page-title">{t('options_title')}</h1>
      <p className="page-subtitle">
        {t('options_subtitle')}{session.isAdmin ? t('options_subtitleUsers') : ''}
      </p>

      <div className="card" style={{ maxWidth: 460, marginBottom: 20 }}>
        <h2 style={{ fontSize: 18, marginBottom: 14 }}>{t('options_appearance')}</h2>
        <div style={{ display: 'flex', gap: 10, marginBottom: 16 }}>
          <button className={`nav-btn ${theme === 'dark' ? 'active' : ''}`} style={{ flex: 1, padding: '10px 0' }} onClick={() => onChangeTheme('dark')}>
            🌙 {t('options_night')}
          </button>
          <button className={`nav-btn ${theme === 'light' ? 'active' : ''}`} style={{ flex: 1, padding: '10px 0' }} onClick={() => onChangeTheme('light')}>
            ☀️ {t('options_day')}
          </button>
        </div>

        <label style={{ display: 'block', marginBottom: 8 }}>{t('options_language')}</label>
        <div style={{ display: 'flex', gap: 10 }}>
          <button className={`nav-btn ${language === 'en' ? 'active' : ''}`} style={{ flex: 1, padding: '10px 0' }} onClick={() => onChangeLanguage('en')}>
            English
          </button>
          <button className={`nav-btn ${language === 'es' ? 'active' : ''}`} style={{ flex: 1, padding: '10px 0' }} onClick={() => onChangeLanguage('es')}>
            Español
          </button>
        </div>
      </div>

      <StationNameCard
        stationName={stationName}
        autoPrintInvoice={autoPrintInvoice}
        onChangeStationName={onChangeStationName}
        onChangeAutoPrint={onChangeAutoPrint}
        t={t}
      />

      {session.isAdmin && <CreateUser session={session} t={t} />}
      {session.isAdmin && <UsersPermissions session={session} t={t} />}
    </div>
  )
}

function StationNameCard({ stationName, autoPrintInvoice, onChangeStationName, onChangeAutoPrint, t }) {
  const [value, setValue] = useState(stationName)
  const [autoPrint, setAutoPrint] = useState(autoPrintInvoice)
  const [message, setMessage] = useState(null)

  useEffect(() => setValue(stationName), [stationName])
  useEffect(() => setAutoPrint(autoPrintInvoice), [autoPrintInvoice])

  async function save(e) {
    e.preventDefault()
    setMessage(null)
    try {
      await api.updateSettings(value, autoPrint)
      onChangeStationName(value)
      onChangeAutoPrint(autoPrint)
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

        <div className="form-row" style={{ flexDirection: 'row', alignItems: 'center', gap: 8 }}>
          <input
            type="checkbox"
            id="autoPrintInvoice"
            checked={autoPrint}
            onChange={(e) => setAutoPrint(e.target.checked)}
            style={{ width: 'auto' }}
          />
          <label htmlFor="autoPrintInvoice" style={{ margin: 0 }}>{t('options_autoPrintInvoice')}</label>
        </div>

        <button type="submit" className="btn btn-primary btn-block">{t('save')}</button>
      </form>
    </div>
  )
}

function CreateUser({ session, t }) {
  const [name, setName] = useState('')
  const [username, setUsername] = useState('')
  const [password, setPassword] = useState('')
  const [isAdmin, setIsAdmin] = useState(false)
  const [canModifyPrices, setCanModifyPrices] = useState(false)
  const [message, setMessage] = useState(null)
  const [submitting, setSubmitting] = useState(false)

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
        canModifyPrices,
      })
      setMessage({ type: 'success', text: t('options_userCreated', { username }) })
      setName('')
      setUsername('')
      setPassword('')
      setIsAdmin(false)
      setCanModifyPrices(false)
    } catch (err) {
      setMessage({ type: 'error', text: err.message || t('options_userCreateError') })
    } finally {
      setSubmitting(false)
    }
  }

  return (
    <div className="card" style={{ maxWidth: 460, marginBottom: 20 }}>
      <h2 style={{ fontSize: 18, marginBottom: 14 }}>{t('options_users')}</h2>

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
        <div className="form-row" style={{ flexDirection: 'row', alignItems: 'center', gap: 8 }}>
          <input
            type="checkbox" id="canModifyPrices"
            checked={isAdmin || canModifyPrices}
            disabled={isAdmin}
            onChange={(e) => setCanModifyPrices(e.target.checked)}
            style={{ width: 'auto' }}
          />
          <label htmlFor="canModifyPrices" style={{ margin: 0 }}>{t('options_canModifyPrices')}</label>
        </div>
        <button type="submit" className="btn btn-primary btn-block" disabled={submitting}>
          {submitting ? t('options_creating') : t('options_createUser')}
        </button>
      </form>
    </div>
  )
}

function UsersPermissions({ session, t }) {
  const [employees, setEmployees] = useState([])
  const [message, setMessage] = useState(null)

  useEffect(() => { load() }, [])

  function load() {
    api.getEmployees().then((data) => {
      setEmployees(data.map((e) => ({ ...e, locked: e.isAdmin })))
    }).catch(() => {})
  }

  async function toggle(employee, value) {
    setMessage(null)
    try {
      await api.updateEmployeePermissions(employee.id, session.employeeId, value)
      setMessage({ type: 'success', text: t('options_permissionsUpdated', { username: employee.username }) })
      load()
    } catch (err) {
      setMessage({ type: 'error', text: err.message || t('options_permissionsError') })
    }
  }

  return (
    <div className="card" style={{ maxWidth: 460 }}>
      <h2 style={{ fontSize: 18, marginBottom: 14 }}>{t('options_pricePermissions')}</h2>
      {message && <div className={`alert alert-${message.type}`}>{message.text}</div>}

      {employees.length > 0 && (
        <table style={{ marginBottom: 18 }}>
          <thead>
            <tr>
              <th>{t('options_colName')}</th>
              <th>{t('options_colUsername')}</th>
              <th>{t('options_colAdmin')}</th>
              <th>{t('options_canModifyPrices')}</th>
            </tr>
          </thead>
          <tbody>
            {employees.map((e) => (
              <tr key={e.id}>
                <td>{e.name}</td>
                <td>{e.username}</td>
                <td>{e.isAdmin ? t('yes') : t('no')}</td>
                <td>
                  <input
                    type="checkbox"
                    checked={e.isAdmin || e.canModifyPrices}
                    disabled={e.isAdmin}
                    onChange={(e2) => toggle(e, e2.target.checked)}
                  />
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  )
}