import { useEffect, useState } from 'react'
import { api } from './api'
import { translate } from './i18n'
import Login from './components/Login'
import Dashboard from './components/Dashboard'
import RegisterSale from './components/RegisterSale'
import RegisterPurchase from './components/RegisterPurchase'
import Reports from './components/Reports'
import Prices from './components/Prices'
import Options from './components/Options'

export default function App() {
  const [session, setSession] = useState(null)
  const [screen, setScreen] = useState('dashboard')
  const [theme, setTheme] = useState(() => localStorage.getItem('station-theme') || 'dark')
  const [language, setLanguage] = useState(() => localStorage.getItem('station-language') || 'en')
  const [stationName, setStationName] = useState('Station')
  const [autoPrintInvoice, setAutoPrintInvoice] = useState(false)

  const t = (key, vars) => translate(language, key, vars)

    useEffect(() => {
    document.body.setAttribute('data-theme', theme)
    localStorage.setItem('station-theme', theme)
  }, [theme])

  useEffect(() => {
    localStorage.setItem('station-language', language)
  }, [language])

  useEffect(() => {
    const raw = localStorage.getItem('auth_session')
    if (raw) setSession(JSON.parse(raw))
  }, [])

  useEffect(() => {
    api.getSettings()
      .then((c) => {
        setStationName(c.stationName)
        setAutoPrintInvoice(!!c.autoPrintInvoice)
      })
      .catch(() => {})
  }, [])

    if (!session) {
    return <Login onLogin={(s) => {
      localStorage.setItem('auth_session', JSON.stringify(s))
      setSession(s)
    }} t={t} />
  }

  const SCREENS = [
    { id: 'dashboard', label: t('nav_station') },
    { id: 'sale', label: t('nav_sale') },
    { id: 'purchase', label: t('nav_purchase') },
    { id: 'prices', label: t('nav_prices') },
    { id: 'reports', label: t('nav_reports') },
    { id: 'options', label: t('nav_options') },
  ]

  return (
    <div className="app-shell">
      <header className="topbar">
        <div className="brand">
          <div className="brand-mark" />
          <span className="brand-title">{stationName}</span>
        </div>

        <nav className="nav">
          {SCREENS.map((s) => (
            <button
              key={s.id}
              className={`nav-btn ${screen === s.id ? 'active' : ''}`}
              onClick={() => setScreen(s.id)}
            >
              {s.label}
            </button>
          ))}
        </nav>

                            <div className="user-chip">
            {session.name}
            <button className="logout-btn" onClick={() => {
              api.logout()
              localStorage.removeItem('auth_session')
              setSession(null)
            }}>{t('signOut')}</button>
          </div>
      </header>

      <main>
        {screen === 'dashboard' && <Dashboard stationName={stationName} t={t} />}
        {screen === 'sale' && <RegisterSale session={session} autoPrintInvoice={autoPrintInvoice} stationName={stationName} t={t} />}
        {screen === 'purchase' && <RegisterPurchase session={session} autoPrintInvoice={autoPrintInvoice} stationName={stationName} t={t} />}
        {screen === 'prices' && <Prices session={session} t={t} />}
        {screen === 'reports' && <Reports t={t} />}
        {screen === 'options' && (
          <Options
            session={session}
            theme={theme}
            onChangeTheme={setTheme}
            language={language}
            onChangeLanguage={setLanguage}
            stationName={stationName}
            onChangeStationName={setStationName}
            autoPrintInvoice={autoPrintInvoice}
            onChangeAutoPrint={setAutoPrintInvoice}
            t={t}
          />
        )}
      </main>
    </div>
  )
}
