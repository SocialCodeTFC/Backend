using System.Text.RegularExpressions;

namespace SocialCode.API.Validators
{
    public static class CommonValidator
    {
        private static readonly Regex _checkForHexRegExp = new Regex("^[0-9a-fA-F]{24}$");

        public static bool IsValidId(string id)
        {
            return id is { } && _checkForHexRegExp.IsMatch(id);
        }
    }
}