using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Sixpence.Web.WebApi
{
    public class SlugifyParameterTransformer : IOutboundParameterTransformer, IParameterPolicy
    {
        public string TransformOutbound(object value)
        {
            if (value != null)
            {
                return Regex.Replace(value.ToString(), "([a-z])([A-Z])", "$1_$2").ToLower();
            }

            return null;
        }
    }
}
