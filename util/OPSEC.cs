using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ___.util
{
    class OPSEC
    {
        public static Random _random = new Random();
        //generate a number up to 30
        public static int nLength = _random.Next(30);

        // Generates a random string with a given size.    
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[_random.Next(s.Length)]).ToArray());
        }

        public static string GenerateTitle()
        {
            var string1 = RandomString(nLength);
            return string1;
        }

    }
}
