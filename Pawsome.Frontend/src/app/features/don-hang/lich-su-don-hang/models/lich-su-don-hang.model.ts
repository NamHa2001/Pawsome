export type OrderStatus =
  | 'cho_xu_ly' | 'dang_xu_ly' | 'da_giao_van' | 'da_giao'
  | 'da_huy' | 'cho_tra_hang' | 'da_tra_hang';

export const ORDER_STATUS_LABELS: Record<OrderStatus, string> = {
  cho_xu_ly: 'Pending', dang_xu_ly: 'Processing', da_giao_van: 'Shipped',
  da_giao: 'Delivered', da_huy: 'Cancelled',
  cho_tra_hang: 'Return Requested', da_tra_hang: 'Returned'
};

export const ORDER_STATUS_CSS_CLASS: Record<OrderStatus, string> = {
  cho_xu_ly: 'StatusPending', dang_xu_ly: 'StatusProcessing', da_giao_van: 'StatusShipped',
  da_giao: 'StatusDelivered', da_huy: 'StatusCancelled',
  cho_tra_hang: 'StatusReturnPending', da_tra_hang: 'StatusReturned'
};

export const ORDER_STATUS_FILTERS: { value: OrderStatus | ''; label: string }[] = [
  { value: '', label: 'All Orders' },
  { value: 'cho_xu_ly', label: 'Pending' },
  { value: 'dang_xu_ly', label: 'Processing' },
  { value: 'da_giao_van', label: 'Shipped' },
  { value: 'da_giao', label: 'Delivered' },
  { value: 'cho_tra_hang', label: 'Return Requested' },
  { value: 'da_tra_hang', label: 'Returned' },
  { value: 'da_huy', label: 'Cancelled' }
];

const TRANG_THAI_CHO_HUY = new Set<string>(['cho_xu_ly', 'dang_xu_ly']);
export function canCancelOrder(t: string): boolean { return TRANG_THAI_CHO_HUY.has(t); }
export function canRequestReturn(t: string): boolean { return t === 'da_giao'; }
export function orderStatusLabel(t: string): string { return ORDER_STATUS_LABELS[t as OrderStatus] ?? t; }
export function orderStatusCssClass(t: string): string { return ORDER_STATUS_CSS_CLASS[t as OrderStatus] ?? 'StatusPending'; }