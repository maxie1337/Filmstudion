using System;
using API.Interfaces;
namespace API.Models.User;

public class UserRegisterDto : IUserRegister
{
    public string Username { get; set; }
    public string Password { get; set; }
    public bool IsAdmin { get; set; }
}
