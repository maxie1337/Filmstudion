using System;
using API.Interfaces;

namespace API.Models.User;

  public class UserAuthenticateDto : IUserAuthenticate
{
    public string Username { get; set; }
    public string Password { get; set; }
}