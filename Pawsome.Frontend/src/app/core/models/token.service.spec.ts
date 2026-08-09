import { TestBed } from '@angular/core/testing';
import { TokenService } from './token.service';
import { AuthUser } from './auth-user.model';

describe('TokenService', () => {
  let service: TokenService;

  const admin: AuthUser = {
    token: 'fake-token',
    userId: 1,
    email: 'admin@pawsome.vn',
    hoTen: 'Quản trị viên',
    role: 'Admin'
  };

  beforeEach(() => {
    localStorage.clear();
    TestBed.configureTestingModule({});
    service = TestBed.inject(TokenService);
  });

  it('chưa đăng nhập thì currentUser$ và các hàm đọc đều trả về rỗng', () => {
    expect(service.getUser()).toBeNull();
    expect(service.isLoggedIn()).toBe(false);
  });

  it('setUser phải lưu vào localStorage VÀ phát tín hiệu qua currentUser$ ngay lập tức', () => {
    const nhanDuoc: (AuthUser | null)[] = [];
    service.currentUser$.subscribe(u => nhanDuoc.push(u));

    service.setUser(admin);

    expect(nhanDuoc).toEqual([null, admin]); // gia tri ban dau (null) + gia tri moi vua set
    expect(service.getUser()).toEqual(admin);
    expect(service.isLoggedIn()).toBe(true);
    expect(JSON.parse(localStorage.getItem('pawsome_auth')!)).toEqual(admin);
  });

  it('clear phải xóa localStorage VÀ phát tín hiệu null qua currentUser$', () => {
    service.setUser(admin);

    const nhanDuoc: (AuthUser | null)[] = [];
    service.currentUser$.subscribe(u => nhanDuoc.push(u));

    service.clear();

    expect(nhanDuoc).toEqual([admin, null]);
    expect(service.getUser()).toBeNull();
    expect(localStorage.getItem('pawsome_auth')).toBeNull();
  });

  it('hasRole phải đúng theo vai trò hiện tại', () => {
    service.setUser(admin);

    expect(service.hasRole('Admin')).toBe(true);
    expect(service.hasRole('Admin', 'Moderator')).toBe(true);
    expect(service.hasRole('Moderator')).toBe(false);
  });

  it('khởi tạo lại service khi localStorage đã có sẵn dữ liệu (mô phỏng tải lại trang) phải đọc đúng', () => {
    localStorage.setItem('pawsome_auth', JSON.stringify(admin));

    TestBed.resetTestingModule();
    TestBed.configureTestingModule({});
    const serviceMoi = TestBed.inject(TokenService);

    expect(serviceMoi.getUser()).toEqual(admin);
    expect(serviceMoi.isLoggedIn()).toBe(true);
  });
});
