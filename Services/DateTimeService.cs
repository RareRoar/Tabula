using System;
using Tabula.Interfaces;

namespace Tabula.Services
{
    public class DateTimeService : IDateTimeService
    {
        private readonly DateTime _currentDateTime;
        public DateTimeService()
        {
            _currentDateTime = DateTime.Now;
        }

        public DateTime CurrentDateTime { get => _currentDateTime; }

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
