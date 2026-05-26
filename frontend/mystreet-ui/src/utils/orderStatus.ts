export const ORDER_STATUS_LABELS: Record<number, string> = {
  0: 'Pending',
  1: 'Processing',
  2: 'Shipped',
  3: 'Delivered',
  4: 'Cancelled'
};

const ORDER_STATUS_COLORS: Record<number, { bg: string; fg: string }> = {
  0: { bg: '#fef3c7', fg: '#92400e' }, // Pending - amber
  1: { bg: '#dbeafe', fg: '#1e40af' }, // Processing - blue
  2: { bg: '#ede9fe', fg: '#5b21b6' }, // Shipped - purple
  3: { bg: '#d1fae5', fg: '#065f46' }, // Delivered - green
  4: { bg: '#fee2e2', fg: '#991b1b' }  // Cancelled - red
};

export function getOrderStatusLabel(status: number | string): string {
  const n = typeof status === 'string' ? Number(status) : status;
  return ORDER_STATUS_LABELS[n] ?? String(status);
}

export function getOrderStatusStyle(status: number | string): React.CSSProperties {
  const n = typeof status === 'string' ? Number(status) : status;
  const c = ORDER_STATUS_COLORS[n] ?? { bg: '#e5e7eb', fg: '#374151' };
  return {
    backgroundColor: c.bg,
    color: c.fg,
    padding: '2px 10px',
    borderRadius: '12px',
    fontSize: '0.85rem',
    fontWeight: 600,
    display: 'inline-block'
  };
}
