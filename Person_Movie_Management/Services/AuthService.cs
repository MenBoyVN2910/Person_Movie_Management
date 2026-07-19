using System;
using Person_Movie_Management.Models;
using Person_Movie_Management.Repositories;
using BCrypt.Net;
using Person_Movie_Management.Helpers;

namespace Person_Movie_Management.Services
{
    public class AuthService
    {
        private readonly UserRepository _userRepository;

        public AuthService()
        {
            _userRepository = new UserRepository();
        }

        public (bool success, string message) Register(string username, string displayName, string password, string? email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username) || username.Length < 4)
                {
                    return (false, "Tên đăng nhập phải có ít nhất 4 ký tự.");
                }

                if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
                {
                    return (false, "Mật khẩu phải có ít nhất 6 ký tự.");
                }

                if (_userRepository.UsernameExists(username))
                {
                    return (false, "Tên đăng nhập đã tồn tại!");
                }

                string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
                var user = new User
                {
                    Username = username,
                    DisplayName = displayName,
                    Email = email,
                    PasswordHash = passwordHash
                };

                _userRepository.Insert(user);
                return (true, "Đăng ký thành công!");
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi hệ thống: {ex.Message}");
            }
        }

        public (bool success, User? user, string message) Login(string username, string password)
        {
            try
            {
                var user = _userRepository.GetByUsername(username);
                if (user == null || !user.IsActive)
                {
                    return (false, null, "Tài khoản không tồn tại hoặc đã bị khóa.");
                }

                if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                {
                    return (false, null, "Mật khẩu không chính xác.");
                }

                SessionManager.Login(user);
                return (true, user, "Đăng nhập thành công!");
            }
            catch (Exception ex)
            {
                return (false, null, $"Lỗi hệ thống: {ex.Message}");
            }
        }
        
        public (bool success, string message) ChangePassword(int userId, string oldPassword, string newPassword)
        {
            try
            {
                var user = _userRepository.GetById(userId);
                if (user == null) return (false, "Tài khoản không tồn tại.");
                
                if (!BCrypt.Net.BCrypt.Verify(oldPassword, user.PasswordHash))
                {
                    return (false, "Mật khẩu cũ không chính xác.");
                }
                
                if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
                {
                    return (false, "Mật khẩu mới phải có ít nhất 6 ký tự.");
                }
                
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
                _userRepository.Update(user);
                return (true, "Đổi mật khẩu thành công!");
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi hệ thống: {ex.Message}");
            }
        }
    }
}
