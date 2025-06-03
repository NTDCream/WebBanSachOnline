using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebBanSachOnline.Models.Tokens
{
    public class Token
    {
        public string Email { get; set; }
        public string Code { get; set; }
        public DateTime ExpiredAt { get; set; }

        public Token(string email, string code)
        {
            Email = email;
            Code = code;
            ExpiredAt = DateTime.Now.AddMinutes(10);
        }
    }
}