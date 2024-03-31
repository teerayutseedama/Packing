using ClosedXML.Excel;
using Packing.Views.DataView;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace Packing.Function
{
    public static class SystemClass
    {
        public static DateTime ConvertTime(string time)
        {
            string[] str=time.Split(":");
            DateTime date = new DateTime();
            if (str.Length > 1)
            {
                date= new DateTime(2000,1,1, int.Parse(str[0]), int.Parse(str[1]), 0);
            }
           
            return date;
        }
        public static DateTime ConvertTimeEnd(string timeend,string timestart)
        {
            string[] str = timeend.Split(":");
            string[] str1 = timestart.Split(":");
            DateTime date = new DateTime();
            if (str.Length > 1)
            {
                date = new DateTime(2000, 1, 1, int.Parse(str[0]), int.Parse(str[1]), 0);
            }
            if (int.Parse(str[0]) < int.Parse(str1[0]))
            {
                date = date.AddDays(1);
            }
            return date;
        }
        public static DateTime ToTimeStampTz(this DateTime dateTime)
        {
            return TimeZoneInfo.ConvertTimeBySystemTimeZoneId(dateTime, "Dateline Standard Time");
        }
    }
    
   


}
