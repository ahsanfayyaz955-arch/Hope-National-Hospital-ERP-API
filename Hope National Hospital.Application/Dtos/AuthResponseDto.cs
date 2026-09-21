using System;

namespace Hope_National_Hospital.Application.Dtos
{
    public class AuthResponseDto
    {
        public bool Success { get; set; }

        public string Message { get; set; } = string.Empty;

        public string? Token { get; set; }

        public string? RefreshToken { get; set; }

        public DateTime? Expiration { get; set; }
    }
}