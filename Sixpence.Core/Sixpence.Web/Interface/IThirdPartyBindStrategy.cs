using System;
using System.Collections.Generic;
using System.Text;

namespace Sixpence.Web
{
    public interface IThirdPartyBindStrategy
    {
        string GetName();
        void Bind(string code, string userid);
    }
}
