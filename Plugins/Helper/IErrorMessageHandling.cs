using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugins.Helper
{
    public interface IErrorMessageHandling
    {
        string ErrorMessage(string error);
    }
}
