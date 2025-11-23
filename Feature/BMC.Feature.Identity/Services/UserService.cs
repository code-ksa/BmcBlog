using System;
using Sitecore.Security.Accounts;

namespace BMC.Feature.Identity.Services
{
    public class UserService
    {
        private const string DomainName = "extranet";

        public bool CreateUser(string username, string email, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                return false;

            try
            {
                var fullUsername = DomainName + "\\" + username;

                var existingUser = User.FromName(fullUsername, false);
                if (existingUser != null && existingUser.IsAuthenticated)
                {
                    return false;
                }

                var user = User.Create(fullUsername, password);
                if (user != null)
                {
                    user.Profile.Email = email;
                    user.Profile.Save();
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        public bool ValidateUser(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return false;

            try
            {
                var fullUsername = DomainName + "\\" + username;
                var user = User.FromName(fullUsername, false);

                if (user == null || !user.IsAuthenticated)
                {
                    return Sitecore.Security.Authentication.AuthenticationManager.Login(fullUsername, password);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool SendPasswordResetEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                return false;

            try
            {
                var users = UserManager.GetUsers();
                foreach (var user in users)
                {
                    if (user.Profile.Email.Equals(email, StringComparison.OrdinalIgnoreCase))
                    {
                        // TODO: Implement email sending logic
                        // Generate reset token and send email
                        return true;
                    }
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        public User GetUserByUsername(string username)
        {
            if (string.IsNullOrEmpty(username))
                return null;

            try
            {
                var fullUsername = DomainName + "\\" + username;
                return User.FromName(fullUsername, false);
            }
            catch
            {
                return null;
            }
        }
    }
}