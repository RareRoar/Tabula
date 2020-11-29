using System.Collections.Generic;

namespace Tabula.Interfaces
{
    public interface IAppDbInitData
    {
        List<string> RolesList { get; set; }
        string AdminName { get; set; }
        string AdminEmail { get; set; }
        string AdminPassword { get; set; }
    }
}
