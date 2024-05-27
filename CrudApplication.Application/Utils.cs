using System.Text;
using System.Text.RegularExpressions;


namespace CrudApplication.Application
{
    public static class Utils
    {
        public static string GetMD5Hash(string @string)
        {
            byte[] inputBytes = Encoding.ASCII.GetBytes(@string);
            byte[] hashBytes = System.Security.Cryptography.MD5.HashData(inputBytes);
            return Convert.ToHexString(hashBytes);
        }

        public static bool IsValidLogin(string login)
        {
            return IsValidPassword(login);
        }

        public static bool IsValidName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }
            Regex regex = new Regex("[A-Za-zа-яА-Я\\s..]");
            if (regex.Count(name) != name.Length)
            {
                return false;
            }
            return true;
        }

        public static bool IsValidPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                return false;
            }
            Regex regex = new Regex("[A-Za-z0-9]");
            if (regex.Count(password) != password.Length)
            {
                return false;
            }
            return true;
        }
    }
}