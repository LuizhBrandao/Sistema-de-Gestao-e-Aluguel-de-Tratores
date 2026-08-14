import React from 'react';

interface Thresholds {
  dangerAbove?: number;
  warningAbove?: number;
  dangerBelow?: number;
  warningBelow?: number;
}

interface GaugeProps {
  value: number;
  max: number;
  unit: string;
  label: string;
  thresholds?: Thresholds;
}

export const Gauge: React.FC<GaugeProps> = ({ value, max, unit, label, thresholds }) => {
  const R = 35;
  const C = 2 * Math.PI * R;
  const pct = Math.min(Math.max(value / max, 0), 1);
  const arc = C * 0.75;
  const offset = arc - (pct * arc);

  let color = 'var(--accent-color)';
  if (thresholds) {
    if (thresholds.dangerAbove != null && value > thresholds.dangerAbove) color = 'var(--critical-color)';
    else if (thresholds.warningAbove != null && value > thresholds.warningAbove) color = 'var(--warning-color)';
    else if (thresholds.dangerBelow != null && value < thresholds.dangerBelow) color = 'var(--critical-color)';
    else if (thresholds.warningBelow != null && value < thresholds.warningBelow) color = 'var(--warning-color)';
  }

  const display = typeof value === 'number' && !Number.isInteger(value) ? value.toFixed(1) : value.toString();

  return (
    <div className="gauge-item">
      <svg className="gauge-svg" viewBox="0 0 100 100">
        <circle 
          className="gauge-track" 
          cx="50" cy="50" r={R}
          strokeDasharray={`${arc} ${C - arc}`} 
          transform="rotate(135 50 50)"
        />
        <circle 
          className="gauge-fill" 
          cx="50" cy="50" r={R}
          stroke={color}
          strokeDasharray={`${arc} ${C - arc}`}
          strokeDashoffset={offset}
          transform="rotate(135 50 50)"
        />
        <text className="gauge-text" x="50" y="46">{display}</text>
        <text className="gauge-unit" x="50" y="60">{unit}</text>
      </svg>
      <span className="gauge-label">{label}</span>
    </div>
  );
};
