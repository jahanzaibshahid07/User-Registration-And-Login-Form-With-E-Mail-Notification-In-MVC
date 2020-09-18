using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace loginform.Models
{
    public static class encryptPassword
    {
        public static string texttoExcript(string password)
        {
            return Convert.ToBase64String(System.Security.Cryptography.SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(password)));
        }
    }
}