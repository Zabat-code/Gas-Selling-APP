// Fuel-needle style gauge — the same symbol everyone recognizes from a
// car's dashboard, applied here to the station's tank.
export default function TankGauge({ percent, color }) {
  const pct = Math.max(0, Math.min(100, percent))
  // Needle goes from -120° (empty) to +120° (full), centered at 0
  const angle = -120 + (pct / 100) * 240

  const cx = 60, cy = 60, r = 46

  const ticks = [0, 25, 50, 75, 100].map((m) => {
    const a = (-120 + (m / 100) * 240) * (Math.PI / 180)
    const x1 = cx + Math.sin(a) * (r - 6)
    const y1 = cy - Math.cos(a) * (r - 6)
    const x2 = cx + Math.sin(a) * r
    const y2 = cy - Math.cos(a) * r
    return <line key={m} x1={x1} y1={y1} x2={x2} y2={y2} stroke="var(--border)" strokeWidth="2" />
  })

  return (
    <svg viewBox="0 0 120 90" width="120" height="90">
      <path
        d="M 14 76 A 46 46 0 1 1 106 76"
        fill="none"
        stroke="var(--surface-raised)"
        strokeWidth="8"
        strokeLinecap="round"
      />
      {ticks}
      <g style={{ transform: `rotate(${angle}deg)`, transformOrigin: `${cx}px ${cy}px`, transition: 'transform 0.6s ease' }}>
        <line x1={cx} y1={cy} x2={cx} y2={cy - r + 14} stroke={color} strokeWidth="3" strokeLinecap="round" />
      </g>
      <circle cx={cx} cy={cy} r="4" fill={color} />
      <text x={cx} y={cy + 26} textAnchor="middle" fontSize="15" fontWeight="700" fill="var(--text)" className="mono-num">
        {Math.round(pct)}%
      </text>
    </svg>
  )
}
