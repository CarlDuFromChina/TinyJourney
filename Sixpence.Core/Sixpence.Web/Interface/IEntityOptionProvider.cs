using System.Collections.Generic;
using Sixpence.Web.Model;

namespace Sixpence.Web
{
    public interface IEntityOptionProvider
    {
        IEnumerable<SelectOption> GetOptions();
    }
}
