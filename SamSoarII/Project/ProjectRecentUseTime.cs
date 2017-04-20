using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.AppMain.Project
{
    public class ProjectRecentUseTime : IComparable<ProjectRecentUseTime>
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
        public int Hour { get; set; }
        public int Minute { get; set; }
        public int Second { get; set; }
        public ProjectRecentUseTime(DateTime time)
        {
            Year = time.Year;
            Month = time.Month;
            Day = time.Day;
            Hour = time.Hour;
            Minute = time.Minute;
            Second = time.Second;
        }
        public ProjectRecentUseTime(int year, int month, int day, int hour, int minute, int second)
        {
            Year = year;
            Month = month;
            Day = day;
            Hour = hour;
            Minute = minute;
            Second = second;
        }
        public int CompareTo(ProjectRecentUseTime other)
        {
            if (Year == other.Year)
            {
                if (Month == other.Month)
                {
                    if (Day == other.Day)
                    {
                        if (Hour == other.Hour)
                        {
                            if (Minute == other.Minute)
                            {
                                return other.Second - Second;
                            }
                            else
                            {
                                return other.Minute - Minute;
                            }
                        }
                        else
                        {
                            return other.Hour - Hour;
                        }
                    }
                    else
                    {
                        return other.Day - Day;
                    }
                }
                else
                {
                    return other.Month - Month;
                }
            }
            else
            {
                return other.Year - Year;
            }
        }
    }
}
