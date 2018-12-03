using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace ReadyApi.AspNetCore.BasicAuth
{
    public class BasicAuthOptions
        : AuthenticationSchemeOptions
    {
        private string _identifier;

        public BasicAuthOptions()
        {
        }

        public string Identifier
        {
            get => _identifier;

            set
            {
                if (!string.IsNullOrEmpty(value) && !IsAscii(value))
                {
                    throw new ArgumentOutOfRangeException("Identifier", "Identifier must be ASCII");
                }

                _identifier = value;
            }
        }

        public bool AllowInsecureProtocol { get; set; }

        private bool IsAscii(string input)
        {
            foreach (char c in input)
            {
                if (c < 32 || c >= 127)
                {
                    return false;
                }
            }

            return true;
        }

        public Func<string, string, ClaimsPrincipal> ExecuteBasicAuthHandler;
    }
}