using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sixpence.Web.WebApi
{
    public class SnakeCaseNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name)
        {
            return ToSnakeCase(name);
        }

        private static string ToSnakeCase(string name)
        {
            if (string.IsNullOrEmpty(name)) return name;

            var stringBuilder = new StringBuilder();
            bool wasPrevUpper = false;

            for (int i = 0; i < name.Length; i++)
            {
                char c = name[i];
                if (char.IsUpper(c))
                {
                    if (stringBuilder.Length > 0 && !wasPrevUpper)
                    {
                        stringBuilder.Append('_');
                    }

                    stringBuilder.Append(char.ToLowerInvariant(c));
                    wasPrevUpper = true;
                }
                else
                {
                    stringBuilder.Append(c);
                    wasPrevUpper = false;
                }
            }

            return stringBuilder.ToString();
        }
    }

}
