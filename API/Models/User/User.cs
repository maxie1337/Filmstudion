using System;
using API.Interfaces;

namespace API.Models.User;

public class User : IUser
{
    public int UserId { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }  //Kom ihåg att lagra lösenordet säkert, kolla på detta senare, glöm ej.
    public string Role { get; set; }
}
