// Khớp đúng AuthResponseDto bên backend (Pawsome.API/DTOs/TaiKhoan/AuthResponseDto.cs)
export interface AuthUser {
  token: string;
  userId: number;
  email: string;
  hoTen: string;
  role: string;
}
