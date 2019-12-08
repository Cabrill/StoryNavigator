using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StoryNavigator
{
    public static class StringExtensions
    {

        public static bool IsNullOrWhitespace(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        public static bool IsNumeric(this string value)
        {
            return !value.IsNullOrWhitespace() && Regex.IsMatch(value, @"^\d+$");
        }
    }
}
