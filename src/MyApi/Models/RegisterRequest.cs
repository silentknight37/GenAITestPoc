namespace MyApi.Models;

public record RegisterRequest(string Email, string Password, int Age);
public record LoginRequest(string Email, string Password);

