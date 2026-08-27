export interface PawPointsBalance {
  userId: number;
  soDuHienTai: number;
}

export interface PawPointsTransaction {
  transactionId: number;
  orderId: number | null;
  soDiem: number; // dương = cộng (earn/bonus), âm = trừ/hoàn (redeem)
  loai: 'earn' | 'redeem' | 'bonus';
  ngayGiaoDich: string;
}

export interface PawPointsHistoryFilter {
  loai?: string;
  page?: number;
  pageSize?: number;
}

export const VND_PER_PAWPOINT = 1000;

export const PAWPOINTS_TYPE_LABELS: Record<string, string> = {
  earn: 'Earned from Order',
  redeem: 'Redeemed for Discount',
  bonus: 'Bonus'
};

export const PAWPOINTS_TYPE_FILTERS: { value: string; label: string }[] = [
  { value: '', label: 'All' },
  { value: 'earn', label: 'Earned' },
  { value: 'redeem', label: 'Redeemed' },
  { value: 'bonus', label: 'Bonus' }
];

export function pawPointsTypeLabel(loai: string): string {
  return PAWPOINTS_TYPE_LABELS[loai] ?? loai;
}