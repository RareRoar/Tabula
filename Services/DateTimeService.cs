using System;
using Tabula.Interfaces;

namespace Tabula.Services
{
    public class DateTimeService : IDateTimeService
    {
        public DateTimeService()
        {
            CurrentDateTime = DateTime.Now;
        }

        public DateTime CurrentDateTime { get; set; }

        public string GetDateComment()
        {
            int mounth = CurrentDateTime.Month;
            if (mounth == 12 || mounth == 1)
            {
                return "Merry Christmas and a Happy New Year!";
            }
            return "";
        }

        public string GetDateTimeString()
        {
            return CurrentDateTime.ToString("MM.dd.yyyy");
        }
    }
}
