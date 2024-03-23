using ClosedXML.Excel;
using Packing.Views.DataView;
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
                date= new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day, int.Parse(str[0]), int.Parse(str[1]), 0);
            }
           
            return date;
        }
    }

 
}
