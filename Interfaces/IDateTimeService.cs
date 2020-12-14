using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tabula.Interfaces
{
    public interface IDateTimeService
    {
        DateTime CurrentDateTime { get; }
        string GetDateTimeString();
        string GetDateComment();
    }
}
