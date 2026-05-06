using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RetailAPI.Data;
using RetailAPI.DTOs;
using RetailAPI.Services;

namespace RetailAPI.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class OtpController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;

        public OtpController(AppDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // 🔹 1. Send OTP
        [HttpPost("send-otp")]
        public async Task<IActionResult> SendOtp([FromBody] SendOtpDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == dto.Email);

            // Don't reveal if email exists
            if (user == null) return Ok();

            var otp = new Random().Next(100000, 999999).ToString();

            user.ResetOtp = otp;
            user.OtpExpiry = DateTime.UtcNow.AddMinutes(10);

            await _context.SaveChangesAsync();

            await _emailService.SendEmailAsync(
                dto.Email!,
                "Password Reset OTP",
                $"Your OTP is: <b>{otp}</b> (valid for 10 minutes)"
            );

            return Ok();
        }

        // 🔹 2. Verify OTP + Reset Password
        [HttpPost("verify-reset")]
        public async Task<IActionResult> VerifyReset([FromBody] VerifyResetDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == dto.Email);

            if (user == null ||
                user.ResetOtp != dto.Otp ||
                user.OtpExpiry < DateTime.UtcNow)
            {
                return BadRequest("Invalid or expired OTP");
            }

            user.Password = dto.NewPassword;

            user.ResetOtp = null;
            user.OtpExpiry = null;

            await _context.SaveChangesAsync();

            return Ok("Password reset successful");
        }
    }
}