
using System.Security.Cryptography;
using System.Text;

namespace AuthorizationsSso.Helpers;

public class AccessCodeHelper
{
    public static string GenerateAccessCode()
    {
        string code = "";

        for (int i = 0; i < 26; i++)
        {
            code += Guid.NewGuid().ToString();
        }

        code = code.Replace("-","");

        return code;
    }
}