import { Observable, map, of } from 'rxjs';
import { CommonModule } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { ChatAi } from '../../../shared/components/chat-ai/chat-ai';
import { Footer } from '../../../shared/components/footer/footer';
import { Header } from '../../../shared/components/header/header';
import { CartService } from '../../gio-hang/gio-hang/services/cart.service';
import { CouponService } from '../../gio-hang/gio-hang/services/coupon.service';
import { Cart } from '../../gio-hang/gio-hang/models/gio-hang.model';
import { Address, CreateAddressRequest } from './models/address.model';
import {
  CreateOrderRequest,
  PaymentMethod,
  SHIPPING_CARRIER_LABELS,
  ShippingCarrier
} from './models/thanh-toan.model';
import { AddressService } from './services/address.service';
import { OrderService } from './services/order.service';
import { PaymentService } from './services/payment.service';
import { PawPointsService } from '../pawpoints/services/pawpoints.service';
import { VND_PER_PAWPOINT } from '../pawpoints/models/pawpoints.model';

@Component({
  selector: 'app-thanh-toan',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, Header, Footer, ChatAi],
  templateUrl: './thanh-toan.html',
  styleUrl: './thanh-toan.css'
})
export class ThanhToanComponent {
  private readonly cartService = inject(CartService);
  private readonly couponService = inject(CouponService);
  private readonly addressService = inject(AddressService);
  private readonly orderService = inject(OrderService);
  private readonly paymentService = inject(PaymentService);
  private readonly pawPointsService = inject(PawPointsService);
  private readonly router = inject(Router);

  readonly cart = signal<Cart | null>(null);
  readonly loading = signal(true);
  readonly errorMessage = signal<string | null>(null);

  readonly addresses = signal<Address[]>([]);
  readonly selectedAddressId = signal<number | null>(null);
  readonly showAddAddressForm = signal(false);
  readonly savingAddress = signal(false);

  // New-address form fields (kept simple, matches CreateAddressRequestDto)
  newRecipientName = '';
  newPhone = '';
  newAddressDetail = '';
  newWard = '';
  newDistrict = '';
  newProvince = '';
  newSetAsDefault = false;

  readonly shippingCarrierLabels = SHIPPING_CARRIER_LABELS;
  readonly shippingCarriers: ShippingCarrier[] = ['GHN', 'GHTK', 'ViettelPost'];
  readonly selectedCarrier = signal<ShippingCarrier>('GHN');

  readonly selectedPaymentMethod = signal<PaymentMethod>('momo');
  readonly placingOrder = signal(false);

  // PawPoints (dùng điểm để giảm giá ngay lúc đặt hàng)
  readonly pawPointsBalance = signal(0);
  readonly usePawPoints = signal(false);
  readonly pawPointsToUse = signal(0);
  readonly vndPerPoint = VND_PER_PAWPOINT;

  /* Không cho dùng nhiều điểm hơn số dư, và không cho dùng nhiều hơn mức backend
   thực sự áp dụng được: OrderService.cs giới hạn (giamGiaCoupon + giamGiaDiem) <= tienHang,
   nên phần điểm tối đa còn hữu ích = (tienHang - giamGiaCoupon) / 10.000 */
  readonly maxUsablePoints = computed(() => {
    const cart = this.cart();
    if (!cart) return 0;
    const conLaiSauCoupon = Math.max(0, cart.tienHang - cart.giamGia);
    const gioiHanTheoDonHang = Math.floor(conLaiSauCoupon / this.vndPerPoint);
    return Math.min(this.pawPointsBalance(), gioiHanTheoDonHang);
  });

  readonly pawPointsDiscount = computed(() =>
    this.usePawPoints() ? this.pawPointsToUse() * this.vndPerPoint : 0);

  readonly finalTotal = computed(() => {
    const cart = this.cart();
    if (!cart) return 0;
    return Math.max(0, cart.tongTien - this.pawPointsDiscount());
  });

  readonly isCartEmpty = computed(() => !this.cart() || this.cart()!.items.length === 0);

  constructor() {
    this.loadCart();
    this.loadAddresses();
    this.loadPawPointsBalance();
  }

  private loadCart(): void {
    this.loading.set(true);
    this.cartService.layGioHang().subscribe({
      next: res => {
        this.cart.set(res.data);
        this.loading.set(false);
      },
      error: () => {
        this.errorMessage.set('Could not load your cart. Please try again.');
        this.loading.set(false);
      }
    });
  }

  private loadAddresses(): void {
    this.addressService.getAll().subscribe({
      next: list => {
        this.addresses.set(list);
        const defaultAddress = list.find(a => a.laMacDinh) ?? list[0];
        if (defaultAddress) this.selectedAddressId.set(defaultAddress.addressId);
        if (list.length === 0) this.showAddAddressForm.set(true);
      },
      error: () => this.errorMessage.set('Could not load your saved addresses.')
    });
  }

  selectAddress(addressId: number): void {
    this.selectedAddressId.set(addressId);
    this.showAddAddressForm.set(false);
  }

  toggleAddAddressForm(): void {
    this.showAddAddressForm.set(!this.showAddAddressForm());
  }

  saveNewAddress(): void {
    if (!this.newRecipientName.trim() || !this.newPhone.trim() ||
        !this.newAddressDetail.trim() || !this.newProvince.trim()) {
      this.errorMessage.set('Please fill in recipient name, phone, address and province.');
      return;
    }

    const dto: CreateAddressRequest = {
      nguoiNhan: this.newRecipientName.trim(),
      soDienThoai: this.newPhone.trim(),
      diaChiChiTiet: this.newAddressDetail.trim(),
      phuongXa: this.newWard.trim() || undefined,
      quanHuyen: this.newDistrict.trim() || undefined,
      tinhThanh: this.newProvince.trim(),
      laMacDinh: this.newSetAsDefault
    };

    this.savingAddress.set(true);
    this.errorMessage.set(null);

    this.addressService.create(dto).subscribe({
      next: created => {
        this.savingAddress.set(false);
        this.addresses.set([...this.addresses(), created]);
        this.selectedAddressId.set(created.addressId);
        this.showAddAddressForm.set(false);
      },
      error: err => {
        this.savingAddress.set(false);
        this.errorMessage.set(err?.error?.message ?? 'Could not save the new address.');
      }
    });
  }

  selectCarrier(carrier: ShippingCarrier): void {
    this.selectedCarrier.set(carrier);
  }

  selectPaymentMethod(method: PaymentMethod): void {
    this.selectedPaymentMethod.set(method);
  }

  private loadPawPointsBalance(): void {
    this.pawPointsService.getBalance().subscribe({
      next: res => this.pawPointsBalance.set(res.soDuHienTai),
      error: () => {} // Không chặn checkout nếu chỉ lỗi lấy số dư điểm - im lặng bỏ qua, coi như 0 điểm
    });
  }

  togglePawPoints(): void {
    const next = !this.usePawPoints();
    this.usePawPoints.set(next);
    // Bật lên thì mặc định đề xuất dùng tối đa; tắt đi thì reset về 0
    this.pawPointsToUse.set(next ? this.maxUsablePoints() : 0);
  }

  onPawPointsInputChange(event: Event): void {
    const raw = Number((event.target as HTMLInputElement).value);
    const value = Number.isFinite(raw) ? Math.trunc(raw) : 0;
    const clamped = Math.max(0, Math.min(value, this.maxUsablePoints()));
    this.pawPointsToUse.set(clamped);
  }

  placeOrder(): void {
    if (!this.selectedAddressId()) {
      this.errorMessage.set('Please select or add a shipping address.');
      return;
    }
    if (this.isCartEmpty()) {
      this.errorMessage.set('Your cart is empty.');
      return;
    }

    this.placingOrder.set(true);
    this.errorMessage.set(null);

    this.resolveCouponId().subscribe(couponId => {
      const dto: CreateOrderRequest = {
        addressId: this.selectedAddressId()!,
        couponId,
        donViVanChuyen: this.selectedCarrier(),
        soDiemMuonDoi: this.usePawPoints() && this.pawPointsToUse() > 0 ? this.pawPointsToUse() : null
      };

      this.orderService.create(dto).subscribe({
        next: order => this.startPayment(order.orderId),
        error: err => {
          this.placingOrder.set(false);
          this.errorMessage.set(err?.error?.message ?? 'Could not place the order. Please try again.');
        }
      });
    });
  }

  private resolveCouponId(): Observable<number | null> {
  const appliedCode = this.cart()?.maCouponDangApDung;
  if (!appliedCode) return of(null);

  return this.couponService.layDangHieuLuc().pipe(
    map(list => list.find(c => c.maCode === appliedCode)?.couponId ?? null)
  );
}

  private startPayment(orderId: number): void {
    const create = this.selectedPaymentMethod() === 'momo'
      ? this.paymentService.createMoMoPayment(orderId)
      : this.paymentService.createVnPayPayment(orderId);

    create.subscribe({
      next: result => {
        window.location.href = result.payUrl;
      },
      error: err => {
        this.placingOrder.set(false);
        this.errorMessage.set(
          err?.error?.message ?? 'Order was created, but starting payment failed. Please try again from Order History.'
        );
      }
    });
  }
}