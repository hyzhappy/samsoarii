using SamSoarII.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SamSoarII.Global
{
    public class ProjectShowMessage : INotifyPropertyChanged
    {
        public List<string> ProjectUsedTimeMessage
        {
            get
            {
                List<string> templist = new List<string>();
                int cnt = 1;
                foreach (var item in ProjectFileManager.RecentUsedProjectMessages)
                {
                    templist.Add(string.Format("{0}:{1}", cnt, StringHelper.Trunc(item.Value.Item2)));
                    cnt++;
                }
                return templist;
            }
        }
        public ProjectShowMessage()
        {
            PropertyChanged = delegate { };
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged()
        {
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs("ProjectUsedTimeMessage"));
        }
    }
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
    public class ProjectFileManager
    {
        private static int MaxCapacity = 10;
        public static SortedDictionary<ProjectRecentUseTime, Tuple<string, string>> RecentUsedProjectMessages;
        public static ProjectShowMessage projectShowMessage;
        static ProjectFileManager()
        {
            RecentUsedProjectMessages = new SortedDictionary<ProjectRecentUseTime, Tuple<string, string>>();
            projectShowMessage = new ProjectShowMessage();
        }
        public static void Update(string projectName, string projectFullName)
        {
            if (RecentUsedProjectMessages.Count == MaxCapacity)
            {
                Delete(MaxCapacity - 1);
            }
            foreach (var item in new Dictionary<ProjectRecentUseTime, Tuple<string, string>>(RecentUsedProjectMessages))
            {
                if (item.Value.Item2 == projectFullName)
                {
                    RecentUsedProjectMessages.Remove(item.Key);
                    break;
                }
            }
            RecentUsedProjectMessages.Add(new ProjectRecentUseTime(DateTime.Now), new Tuple<string, string>(projectName, projectFullName));
            projectShowMessage.RaisePropertyChanged();
        }
        public static void Delete(int index)
        {
            RecentUsedProjectMessages.Remove(RecentUsedProjectMessages.ElementAt(index).Key);
            projectShowMessage.RaisePropertyChanged();
        }
        public static void Delete(ProjectRecentUseTime key)
        {
            RecentUsedProjectMessages.Remove(key);
            projectShowMessage.RaisePropertyChanged();
        }
        public static void Clear()
        {
            RecentUsedProjectMessages.Clear();
        }
        public static void LoadRecentUsedProjectsByXElement(XElement rootNode)
        {
            Clear();
            foreach (var ele in rootNode.Elements())
            {
                ProjectRecentUseTime time = new ProjectRecentUseTime(int.Parse(ele.Element("Year").Value), int.Parse(ele.Element("Month").Value), int.Parse(ele.Element("Day").Value), int.Parse(ele.Element("Hour").Value), int.Parse(ele.Element("Minute").Value), int.Parse(ele.Element("Second").Value));
                Tuple<string, string> tuple = new Tuple<string, string>(ele.Element("ProjectName").Value, ele.Element("ProjectFullFileName").Value);
                RecentUsedProjectMessages.Add(time, tuple);
            }
            projectShowMessage.RaisePropertyChanged();
        }
        public static XElement CreateXElementByRecentUsedProjects()
        {
            XElement rootNode = new XElement("RecentUsedProjectMessages");
            foreach (var item in RecentUsedProjectMessages)
            {
                rootNode.Add(CreateXElementByRecentUsedProject(item));
            }
            return rootNode;
        }
        private static XElement CreateXElementByRecentUsedProject(KeyValuePair<ProjectRecentUseTime, Tuple<string, string>> pair)
        {
            XElement rootNode = new XElement("RecentUsedProjectMessage");
            rootNode.Add(new XElement("Year", pair.Key.Year));
            rootNode.Add(new XElement("Month", pair.Key.Month));
            rootNode.Add(new XElement("Day", pair.Key.Day));
            rootNode.Add(new XElement("Hour", pair.Key.Hour));
            rootNode.Add(new XElement("Minute", pair.Key.Minute));
            rootNode.Add(new XElement("Second", pair.Key.Second));
            rootNode.Add(new XElement("ProjectName", pair.Value.Item1));
            rootNode.Add(new XElement("ProjectFullFileName", pair.Value.Item2));
            return rootNode;
        }
    }
}
