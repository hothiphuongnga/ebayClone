namespace ebay.Controllers
{
    using ebay.Helper;
    using AutoMapper;
    using ebay.Data;
    using ebay.Dtos;
    using ebay.Models;
    using Microsoft.AspNetCore.Mvc;
    using ebay.Base;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.AspNetCore.Authorization;

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IMapper _map, EBayDbContext _context, IJwtAuthService _jwt) : ControllerBase
    {
        [HttpGet]
        // đăng nhâp rồi mới gọi đươc => có token và token hợp lệ 
        // còn hạn sử dụng và nó là token do server tạo ra
        // [Authorize] // yêu cầu xác thực authen
        [Authorize()] // yêu cầu phân quyền role admin mới đươc gọi
        public async Task<IActionResult> Get()
        {

            return Ok();
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDTO dto)
        {
            // kiểm tra username hoặc email đã tồn tại chưa
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == dto.Username || u.Email == dto.Email);
            if (existingUser != null)
            {
                return ResponseEntity<UserDTO>.Fail("Username hoặc Email đã tồn tại");
            }




            // ++++++++++ CÁCH 1 dùng EF
            // tạo user mới thì chuyênr dto về User sau đó dùng _ contexxt add vào database
            // User newUser = _map.Map<User>(dto);
            // // băm mật khẩu
            // newUser.PasswordHash = PasswordHelper.HashPassword(dto.Password);
            // newUser.Deleted = false;
            // // thêm user role
            // var userRole = new UserRole
            // {
            //     UserId = newUser.Id,
            //     RoleId = 9 // role user
            // };
            // newUser.UserRoles.Add(userRole);
            // // lưu user vào database
            // await _context.Users.AddAsync(newUser);
            // // lưu thay đổi
            // await _context.SaveChangesAsync();
            // // trả về kết quả UserDTO
            // var res = _map.Map<UserDTO>(newUser);
            // return ResponseEntity<UserDTO>.Ok(res, "Đăng ký thành công");


            // ========== CÁCH 2 DÙNG SQL Raw
            try
            {
                // bắt đầu transaction
                _context.Database.BeginTransaction();
                string sql = "INSERT INTO Users (Username, Email, PasswordHash, FullName) VALUES (@p0, @p1, @p2, @p3)";
                var passwordHash = PasswordHelper.HashPassword(dto.Password);
                await _context.Database.ExecuteSqlRawAsync(sql, dto.Username, dto.Email, passwordHash, dto.FullName);

                // lấy user vừa tạo để lấy Id gán vào UserRole
                string getUserSql = "SELECT * FROM Users WHERE Username = @p0";
                User newUser = await _context.Users.FromSqlRaw(getUserSql, dto.Username).FirstOrDefaultAsync();
                if (newUser == null)
                {
                    throw new Exception("Không tìm thấy user vừa tạo");
                }
                Console.WriteLine("newUser.Id" + newUser.Id);
                // thêm user role
                string insertUserRoleSql = "INSERT INTO UserRole (UserId, RoleId) VALUES (@p0, @p1)";
                await _context.Database.ExecuteSqlRawAsync(insertUserRoleSql, newUser.Id, 9); // role user


                // commit transaction
                _context.Database.CommitTransaction();
                // trả về kết quả UserDTO
                var res = _map.Map<UserDTO>(newUser);
                return ResponseEntity<UserDTO>.Ok(res, "Đăng ký thành công");
            }
            catch (Exception ex)
            {
                // roll back nếu có lỗi
                // rollback : quay về trạng thái trước khi thực hiện giao dịch 
                _context.Database.RollbackTransaction(); // hoàn tác các thay đổi trong transaction
                return ResponseEntity<UserDTO>.Fail("Đăng ký thất bại: " + ex.Message);
            }

        }

        //========== Login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDTO dto)
        {
            // Tim user theo username
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == dto.Username);
            if (user == null)
            {
                return ResponseEntity<UserDTO>.Fail("Không tìm thấy user");
            }
            // kiem tra mat khau
            bool isPasswordValid = PasswordHelper.VerifyPassword(dto.Password, user.PasswordHash);
            // neu khong dung tra ve loi
            if (!isPasswordValid)
            {
                return ResponseEntity<UserDTO>.Fail("Mật khẩu không đúng");
            }
            // tạo token và trả về
            var token = _jwt.GenerateToken(user);
            var res = new UserLoginResponseDTO
            {
                Token = token
            };
            return ResponseEntity<UserLoginResponseDTO>.Ok(res, "Đăng nhập thành công");

            // pnga9 8 7 6
            //string
        }

    }
}

