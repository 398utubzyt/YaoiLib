using System;
using System.Text;

using Terraria.Localization;

namespace YaoiLib
{
    public static class TimeSpanExtensions
    {
        public static string LocalizedDuration(this TimeSpan time, bool abbreviated, bool onlyLargestUnit)
        {
            StringBuilder text = new StringBuilder();
            abbreviated |= !GameCulture.FromCultureName(GameCulture.CultureName.English).IsActive;

            if (time.Days > 0)
            {
                int days = time.Days;
                if (onlyLargestUnit && time.TotalDays > 1.0)
                    days++;

                text.Append(days);
                text.Append(abbreviated ? Language.GetTextValue("Misc.ShortDays") : days == 1 ? " day" : " days");
                if (onlyLargestUnit)
                    return text.ToString();

                text.Append(' ');
            }

            if (time.Hours > 0)
            {
                int hours = time.Hours;
                if (onlyLargestUnit && time.TotalHours > 1.0)
                    hours++;

                text.Append(hours);
                text.Append(abbreviated ? Language.GetTextValue("Misc.ShortHours") : hours == 1 ? " hour" : " hours");

                if (onlyLargestUnit)
                    return text.ToString();

                text.Append(' ');
            }

            if (time.Minutes > 0)
            {
                int mins = time.Minutes;
                if (onlyLargestUnit && time.TotalMinutes > 1.0)
                    mins++;

                text.Append(mins);
                text.Append(abbreviated ? Language.GetTextValue("Misc.ShortMinutes") : mins == 1 ? " minute" : " minutes");

                if (onlyLargestUnit)
                    return text.ToString();

                text.Append(' ');
            }

            text.Append(time.Seconds);
            text.Append(abbreviated ? Language.GetTextValue("Misc.ShortSeconds") : time.Seconds == 1 ? " second" : " seconds");

            return text.ToString();
        }

        public static string LocalizedDuration(this TimeSpan time)
        {
            StringBuilder text = new StringBuilder();

            if (time.Days > 0)
            {
                text.Append(time.Days);
                text.Append(Language.GetTextValue("Misc.ShortDays"));
                text.Append(' ');
            }

            if (time.Hours > 0)
            {
                text.Append(time.Hours);
                text.Append(Language.GetTextValue("Misc.ShortHours"));
                text.Append(' ');
            }

            if (time.Minutes > 0)
            {
                text.Append(time.Minutes);
                text.Append(Language.GetTextValue("Misc.ShortMinutes"));
                text.Append(' ');
            }

            text.Append(time.Seconds);
            text.Append(Language.GetTextValue("Misc.ShortSeconds"));

            return text.ToString();
        }
    }
}
