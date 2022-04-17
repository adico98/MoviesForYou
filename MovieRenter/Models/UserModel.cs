using System;
using System.Collections.Generic;
using System.Text;

namespace MovieRenter.Models
{
    public class UserModel
    {
        public string Username { get; set; }
        public string Firstname { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public bool IsAdmin { get; set; }

        public UserModel(UserModel copyMember)
        {
            Username = copyMember.Username;
            Firstname = copyMember.Firstname;
            LastName = copyMember.LastName;
            Password = copyMember.Password;
            Email = copyMember.Email;
            DateOfBirth = copyMember.DateOfBirth;
            IsAdmin = copyMember.IsAdmin;
        }
        public UserModel()
        {
            Username = "";
            Firstname = "";
            LastName = "";
            Password = "";
            Email = "";
            DateOfBirth = DateTime.Now;
            IsAdmin = false;
        }

        public UserModel(string username, string fName, string lName, string password, string email, DateTime dateOfBirth, bool isAdmin)
        {
            Username = username;
            Firstname = fName;
            LastName = lName;
            Password = password;
            Email = email;
            DateOfBirth = dateOfBirth;
            IsAdmin = isAdmin;
        }
    }
}
