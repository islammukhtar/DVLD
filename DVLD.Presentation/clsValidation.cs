using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DVLD.Presentation
{
    internal class clsValidation
    {
        public static bool IsValidEmail(string email)
        {
            return Regex.IsMatch(
                email,
                 @"^(?!.*\.\.)[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$");
        }
    }
}
