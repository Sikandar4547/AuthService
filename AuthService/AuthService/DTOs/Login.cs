﻿namespace AuthService.DTOs
{
    public class Login
    {
        public string UsernameOrEmail { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
