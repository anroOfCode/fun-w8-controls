using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fusao.Controls
{
    class FriendlyDateStrings
    {
        private static List<StringThresholdPair> _dateTable = null;

        public static string GetString(DateTime date)
        {
            if (_dateTable == null) Initialize();

            TimeSpan span = DateTime.Now - date;
            
            int i = _dateTable.Count - 1;
            while (true)
            {
                if (span >= _dateTable[i].Threshold)
                {
                    break;
                }
                i--;
            }

            return String.Format(_dateTable[i].FormatString, 
                span.TotalSeconds, 
                span.TotalMinutes, 
                span.TotalHours, 
                span.TotalDays, 
                span.TotalDays / 30, 
                span.TotalDays / 365);
        }

        public static DateTime GetNextUpdateTime(DateTime date)
        {
            if (_dateTable == null) Initialize();

            TimeSpan span = DateTime.Now - date;

            int i = 0;

            while (i < _dateTable.Count - 1)
            {
                if (span < _dateTable[i].Threshold)
                {
                    break;
                }
                i++;
            }

            return DateTime.Now + _dateTable[i].Threshold - span;
        }

        private static void Initialize()
        {
            // Format: 
            // 0: TotalSeconds
            // 1: TotalMinutes
            // 2: TotalHours
            // 3: TotalDays
            // 4: TotalMonths
            // 5: TotalYears

            _dateTable = new List<StringThresholdPair>();

            _dateTable.Add(new StringThresholdPair(
                formatString: "a few moments ago",
                threshold: TimeSpan.FromMilliseconds(0)
                ));

            _dateTable.Add(new StringThresholdPair(
                formatString: "about 15 seconds ago",
                threshold: TimeSpan.FromSeconds(15)
                ));

            _dateTable.Add(new StringThresholdPair(
                formatString: "about 30 seconds ago",
                threshold: TimeSpan.FromSeconds(30)
                ));

            //_dateTable.Add(new StringThresholdPair(
            //    formatString: "4 seconds ago",
            //    threshold: TimeSpan.FromSeconds(4)
            //    ));

            _dateTable.Add(new StringThresholdPair(
                formatString: "about a minute ago",
                threshold: TimeSpan.FromSeconds(59)
                ));

            _dateTable.Add(new StringThresholdPair(
                formatString: "about 2 minutes ago",
                threshold: TimeSpan.FromSeconds(119)
                ));

            _dateTable.Add(new StringThresholdPair(
                formatString: "about 3 minutes ago",
                threshold: TimeSpan.FromSeconds(179)
                ));

            _dateTable.Add(new StringThresholdPair(
                formatString: "about 5 minutes ago",
                threshold: TimeSpan.FromMinutes(5)
                ));

            _dateTable.Add(new StringThresholdPair(
                formatString: "about 10 minutes ago",
                threshold: TimeSpan.FromMinutes(10)
                ));

            _dateTable.Add(new StringThresholdPair(
                formatString: "about 15 minutes ago",
                threshold: TimeSpan.FromMinutes(15)
                ));

            _dateTable.Add(new StringThresholdPair(
                formatString: "about 30 minutes ago",
                threshold: TimeSpan.FromMinutes(30)
                ));

            _dateTable.Add(new StringThresholdPair(
                formatString: "about an hour ago",
                threshold: TimeSpan.FromHours(1)
                ));

            _dateTable.Add(new StringThresholdPair(
                formatString: "about 2 hours ago",
                threshold: TimeSpan.FromHours(2)
                ));

            _dateTable.Add(new StringThresholdPair(
                formatString: "about 3 hours ago",
                threshold: TimeSpan.FromHours(3)
                ));

            _dateTable.Add(new StringThresholdPair(
                formatString: "about 4 hours ago",
                threshold: TimeSpan.FromHours(4)
                ));

            _dateTable.Add(new StringThresholdPair(
                formatString: "about 5 hours ago",
                threshold: TimeSpan.FromHours(5)
                ));

            _dateTable.Add(new StringThresholdPair(
                formatString: "about 6 hours ago",
                threshold: TimeSpan.FromHours(6)
                ));

            _dateTable.Add(new StringThresholdPair(
                formatString: "about 7 hours ago",
                threshold: TimeSpan.FromHours(7)
                ));

            _dateTable.Add(new StringThresholdPair(
                formatString: "about 8 hours ago",
                threshold: TimeSpan.FromHours(8)
                ));

            _dateTable.Add(new StringThresholdPair(
                formatString: "about 9 hours ago",
                threshold: TimeSpan.FromHours(9)
                ));

            _dateTable.Add(new StringThresholdPair(
                formatString: "about 10 hours ago",
                threshold: TimeSpan.FromHours(10)
                ));

            _dateTable.Add(new StringThresholdPair(
                formatString: "about 11 hours ago",
                threshold: TimeSpan.FromHours(11)
                ));

            _dateTable.Add(new StringThresholdPair(
                formatString: "about a half day ago",
                threshold: TimeSpan.FromHours(12)
                ));

            _dateTable.Add(new StringThresholdPair(
                formatString: "about a day ago",
                threshold: TimeSpan.FromDays(1)
                ));

            _dateTable.Add(new StringThresholdPair(
                formatString: "about 2 days ago",
                threshold: TimeSpan.FromDays(2)
                ));

            _dateTable.Add(new StringThresholdPair(
                formatString: "about 3 days ago",
                threshold: TimeSpan.FromDays(3)
                ));

            _dateTable.Add(new StringThresholdPair(
                formatString: "about 4 days ago",
                threshold: TimeSpan.FromDays(4)
                ));

            _dateTable.Add(new StringThresholdPair(
                formatString: "about 5 days ago",
                threshold: TimeSpan.FromDays(5)
                ));

            _dateTable.Add(new StringThresholdPair(
                formatString: "about 6 days ago",
                threshold: TimeSpan.FromDays(6)
                ));

            _dateTable.Add(new StringThresholdPair(
                formatString: "about a week ago",
                threshold: TimeSpan.FromDays(7)
                ));

            _dateTable.Add(new StringThresholdPair(
                formatString: "about 2 weeks ago",
                threshold: TimeSpan.FromDays(14)
                ));

            _dateTable.Add(new StringThresholdPair(
                formatString: "about 3 weeks ago",
                threshold: TimeSpan.FromDays(21)
                ));

        }

        private class StringThresholdPair
        {
            public StringThresholdPair(string formatString, TimeSpan threshold)
            {
                this.FormatString = formatString;
                this.Threshold = threshold;
            }

            public string FormatString { get; private set; }
            public TimeSpan Threshold { get; private set; }
        }
    }
}
