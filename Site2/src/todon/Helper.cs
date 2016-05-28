using Microsoft.AspNet.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace todon
{
    public static class Helper
    {

        #region Converts

        public static String ConvertDateStartToString(String dateStart)
        {
            String[] dateVals = dateStart.Split(' ');
            return dateVals[0];
        }

        public static String ConvertDateStartHourToInt(String dateStart)
        {
            String[] dateVals = dateStart.Split(' ', ':');
            return dateVals[1];
        }

        public static String ConvertPeriodToTime(String period)
        {
            String[] time = period.Split(' ');
            return time[1];
        }

        public static DateTime? ConvertPeriodToDateStart(String period)
        {
            String[] time = period.Split(' ');
            string tempTimeHour = time[1];
            string tempMinutes = time[0];
            DateTime dateStart = DateTime.Now.Date;
            dateStart = dateStart.AddHours(double.Parse(tempTimeHour));
            dateStart = dateStart.AddMinutes(double.Parse(tempMinutes));
            return dateStart;
        }

        public static Int32? ConvertDayToInt32(String day)
        {
            Int32? days = 0;
            if (day == null || day == "") return 0;
            String drx = @"(\d+)?d";
            foreach (Match m in Regex.Matches(day, drx))
            {
                days = Int32.Parse(m.Groups[1].Value);
            }
            return days;
        }

        public static Int32? ConvertDurationToInt32(String duration)
        {
            Int32? mins = 0; Int32 days = 0; Int32 hours = 0;
            if (duration == null || duration == "") return 0;
            String mrx = @"(\d+)?m";
            String hrx = @"(\d+)?h";
            String drx = @"(\d+)?d";

            if (Regex.Matches(duration, mrx).Count > 0)
                mins = Int32.Parse(Regex.Matches(duration, mrx)[0].Groups[1].Value);

            foreach (Match m in Regex.Matches(duration, hrx))
            {
                hours = Int32.Parse(m.Groups[1].Value) * 60;
            }
            foreach (Match m in Regex.Matches(duration, drx))
            {
                days = Int32.Parse(m.Groups[1].Value) * 24 * 60;
            }

            mins = days + hours + mins;
            return mins;
        }

        public static String ConvertMinsToDuration(Int32? minutes)
        {
            String duration = "";
            if (minutes == null) return duration;
            double value = double.Parse(minutes.ToString());
            var d = Math.Floor(value / (60 * 24));

            var h = Math.Floor((value / 60) % 24);
            var m = value - d * 24 * 60 - h * 60;
            if (d > 0) duration = duration + d + "d ";
            if (h > 0) duration = duration + h + "h ";
            if (m > 0) duration = duration + m + "m ";
            if (duration == "")
                duration = "0";
            return duration.Substring(0, duration.Length - 1);
        }

#endregion

        #region UploadFile

        private static string GetFileName(IFormFile file) => file.ContentDisposition.Split(';')
                                                               .Select(x => x.Trim())
                                                               .Where(x => x.StartsWith("filename="))
                                                               .Select(x => x.Substring(9).Trim('"'))
                                                               .First();


        public static bool CheckPictureFileType(string filename)
        {
            var filetypes = new List<String>() { "jpg", "jpeg", "png", "gif" };
            foreach (var t in filetypes)
            {
                if (filename.EndsWith("." + t) || filename.EndsWith("." + t.ToUpper())) return true;
            }
            return false;
        }

        public static String UploadFile(IFormFile file, String webRootPath)
        {
            if (file.Length > 0 && file.Length < 10000000)
            {
                string c = file.ContentType;
                if (file.ContentType.Contains("image"))
                {
                    var targetDirectory = Path.Combine(webRootPath, string.Format("Content\\Uploaded\\"));
                    var fileName = GetFileName(file);
                    if (!string.IsNullOrWhiteSpace(fileName))
                    {
                        if (Helper.CheckPictureFileType(fileName))
                        {
                            var savePath = Path.Combine(targetDirectory, fileName);
                            file.SaveAs(savePath);
                            return fileName;
                        }
                    }
                }
            }
            return String.Empty;
        }

        #endregion
    }
}
