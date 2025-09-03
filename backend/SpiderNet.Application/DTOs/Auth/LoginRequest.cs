﻿namespace SpiderNet.Application.DTOs.Auth;

public class LoginRequest
{
    public string EmailOrUsername { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}