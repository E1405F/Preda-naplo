using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Préda_naplo
{
    internal class Diak
    {
        int[] jegyek;
        string szulo;
        DateTime szuletes;
        string jelszo;
        string email;
        string osztaly;

        public Diak(string szulo, DateTime szuletes, string jelszo, string email, string osztaly)
        {
            this.szulo = szulo;
            this.szuletes = szuletes;
            this.jelszo = jelszo;
            this.email = email;
            this.osztaly = osztaly;
        }

        public void felhasznalo()
        {
            felhasznaloCsekkolas szevasz = new felhasznaloCsekkolas(pinKod);

            if (szevasz.cserel())
            {
                pinKod = szevasz.UJKOD;
            }
        }

        public class felhasznaloCsekkolas
        {
            string jelszo;
            string email;

            public string Jelszo { get { return jelszo; } }
            public string Email { get { return email; } }
            /// <summary>
            /// Very simple email check:
            /// - not empty
            /// - contains one '@'
            /// - has a '.' after the '@' and not at the end
            /// </summary>
            public static bool IsValidEmail(string email)
            {
                if (string.IsNullOrWhiteSpace(email))
                    return false;

                int at = email.IndexOf('@');
                int lastDot = email.LastIndexOf('.');

                return at > 0 && lastDot > at + 1 && lastDot < email.Length - 1 && email.IndexOf('@', at + 1) == -1;
            }

            /// <summary>
            /// Simple password rules:
            /// - at least 6 characters
            /// - contains at least one digit
            /// </summary>
            public static bool IsValidPassword(string password)
            {
                if (string.IsNullOrEmpty(password))
                    return false;

                if (password.Length < 6)
                    return false;

                return password.Any(char.IsDigit);
            }

            /// <summary>
            /// Validate this instance's email and password with simple rules.
            /// Returns true when both are valid; on failure returns false and sets error.
            /// </summary>
            public bool ValidateCredentials(out string error)
            {
                error = null;

                if (!IsValidEmail(this.email))
                {
                    error = "Invalid email.";
                    return false;
                }

                if (!IsValidPassword(this.jelszo))
                {
                    error = "Password must be at least 6 characters and contain a digit.";
                    return false;
                }

                return true;
            }
        }

        
    }
}
