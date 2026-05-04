namespace RetailAPI.DTOs
{
    public class SendOtpDto
    {
        public string? Email { get; set; }
    }

    public class VerifyResetDto
    {
        public string? Email { get; set; }
        public string? Otp { get; set; }
        public string? NewPassword { get; set; }
    }
}
