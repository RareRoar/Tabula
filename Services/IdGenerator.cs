using System;
using Tabula.Interfaces;

namespace Tabula.Services
{
    public class IdGenerator : IUniqueIdGenerator
    {
        public string GenerateUniqueId()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmssffff");
        }
    }
}
