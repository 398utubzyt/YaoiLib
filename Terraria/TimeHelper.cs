using System;
using System.Runtime.CompilerServices;

using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace YaoiLib.Terraria
{
    public delegate void TimeChangeHandler();

    public enum WatchLevel
    {
        None = 0,
        Copper = 1,
        Silver = 2,
        Gold = 3,
    }

    /// <summary>
    /// Provides utility functions and properties for the in-game day/night cycle.
    /// </summary>
    public static class TimeHelper
    {
        public const double DayLengthTicks = Main.dayLength;
        public const double NightLengthTicks = Main.nightLength;

        public const double DayHalfLengthTicks = 27000.0;
        public const double NightHalfLengthTicks = 16200.0;

        public const double DayLengthHours = 15.0;
        public const double NightLengthHours = 9.0;

        public const double DayHalfLengthHours = 7.5;
        public const double NightHalfLengthHours = 4.5;

        public const double HoursPerTick = 0.00027777777777777777;
        public const double TicksPerHour = 3600;

        public const double RealHoursPerTick = 0.00000462962962962963;
        public const double RealMinutesPerTick = 0.00027777777777777778;
        public const double RealSecondsPerTick = 0.01666666666666666667;

        public const double TicksPerRealHour = 216000;
        public const double TicksPerRealMinute = 3600;
        public const double TicksPerRealSecond = 60;

        public const double DayStartHour = 4.5;
        public const double NightStartHour = 19.5;

        internal static void Load()
        {
            On_Main.UpdateTime_StartDay += On_UpdateTime_StartDay;
            On_Main.UpdateTime_StartNight += On_UpdateTime_StartNight;
        }

        internal static void Unload()
        {
            _onBecomeDay = null;
            _onBecomeNight = null;
        }

        private static void On_UpdateTime_StartDay(On_Main.orig_UpdateTime_StartDay orig, ref bool stopEvents)
        {
            orig(ref stopEvents);
            _onBecomeDay?.Invoke();
        }

        private static void On_UpdateTime_StartNight(On_Main.orig_UpdateTime_StartNight orig, ref bool stopEvents)
        {
            orig(ref stopEvents);
            _onBecomeNight?.Invoke();
        }

        private static event TimeChangeHandler _onBecomeDay;
        private static event TimeChangeHandler _onBecomeNight;

        /// <summary>
        /// Invoked after it becomes day time.
        /// </summary>
        public static event TimeChangeHandler OnBecomeDay
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            add => _onBecomeDay += value;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            remove => _onBecomeDay -= value;
        }

        /// <summary>
        /// Invoked after it becomes night time.
        /// </summary>
        public static event TimeChangeHandler OnBecomeNight
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            add => _onBecomeNight += value;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            remove => _onBecomeNight -= value;
        }

        /// <summary>
        /// The current update rate for events in the world.
        /// </summary>
        public static double EventUpdateRate
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Main.desiredWorldEventsUpdateRate;
        }

        /// <summary>
        /// The current update rate for tiles in the world.
        /// </summary>
        public static double TileUpdateRate
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Main.desiredWorldTilesUpdateRate;
        }

        /// <summary>
        /// Converts from game ticks to 24-hour time.
        /// </summary>
        /// <param name="ticks">The ticks to convert.</param>
        /// <param name="isDay">Determines if it is day or night.</param>
        /// <returns>The converted 24-hour time.</returns>
        public static double To24HourFormat(double ticks, bool isDay)
        {
            if (isDay)
                return DayStartHour + ticks * HoursPerTick;

            if (ticks < NightHalfLengthTicks)
                return NightStartHour + ticks * HoursPerTick;

            return (ticks - NightHalfLengthTicks) * HoursPerTick;
        }

        /// <summary>
        /// Converts from 24-hour time to game ticks.
        /// </summary>
        /// <param name="hours">The hours to convert.</param>
        /// <param name="isDay">Determines if it is day or night.</param>
        /// <returns>The converted game ticks.</returns>
        public static double ToTickFormat(double hours, out bool isDay)
        {
            hours %= 24.0;
            if (hours < 0.0)
                hours += 24.0;

            if (hours < DayStartHour)
            {
                isDay = false;
                return NightHalfLengthTicks + (hours * TicksPerHour);
            } else if (hours < NightStartHour)
            {
                isDay = true;
                return (hours - DayStartHour) * TicksPerHour;
            } else
            {
                isDay = false;
                return (hours - NightStartHour) * TicksPerHour;
            }
        }

        /// <summary>
        /// Converts from a game tick duration to real-time hours.
        /// </summary>
        /// <param name="ticks">The ticks to convert.</param>
        /// <returns>The converted hour duration.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double TicksToHours(double ticks)
            => ticks * RealHoursPerTick;

        /// <summary>
        /// Converts from a game tick duration to real-time minutes.
        /// </summary>
        /// <param name="ticks">The ticks to convert.</param>
        /// <returns>The converted minute duration.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double TicksToMinutes(double ticks)
            => ticks * RealMinutesPerTick;

        /// <summary>
        /// Converts from a game tick duration to real-time seconds.
        /// </summary>
        /// <param name="ticks">The ticks to convert.</param>
        /// <returns>The converted second duration.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double TicksToSeconds(double ticks)
            => ticks * RealSecondsPerTick;

        /// <summary>
        /// Converts from a game tick duration to a real-time time span.
        /// </summary>
        /// <param name="ticks">The ticks to convert.</param>
        /// <returns>The converted time span duration.</returns>
        public static TimeSpan TicksToTimeSpan(double ticks)
            => TimeSpan.FromSeconds(ticks * RealSecondsPerTick);

        /// <summary>
        /// Converts from real-time hours to a game tick duration.
        /// </summary>
        /// <param name="hours">The hours to convert.</param>
        /// <returns>The converted game tick duration.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double HoursToTicks(double hours)
            => hours * TicksPerRealHour;

        /// <summary>
        /// Converts from real-time minutes to a game tick duration.
        /// </summary>
        /// <param name="minutes">The minutes to convert.</param>
        /// <returns>The converted game tick duration.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double MinutesToTicks(double minutes)
            => minutes * TicksPerRealMinute;

        /// <summary>
        /// Converts from real-time seconds to a game tick duration.
        /// </summary>
        /// <param name="seconds">The seconds to convert.</param>
        /// <returns>The converted game tick duration.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double SecondsToTicks(double seconds)
            => seconds * TicksPerRealSecond;

        /// <summary>
        /// Converts from real-time time span to a game tick duration.
        /// </summary>
        /// <param name="time">The time span to convert.</param>
        /// <returns>The converted game tick duration.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double TimeSpanToTicks(TimeSpan time)
            => time.TotalSeconds * TicksPerRealSecond;

        /// <summary>
        /// Converts the provided time into a HH:MM formatted string.
        /// </summary>
        /// <returns>The provided time in clock notation.</returns>
        public static string MakeDisplayText(double ticks, bool isDay)
        {
            double hr24 = To24HourFormat(ticks, isDay);
            string amPm = hr24 < 12.0 ? Language.GetTextValue("GameUI.TimeAtMorning") : Language.GetTextValue("GameUI.TimePastMorning");

            int hours = (int)(hr24 % 12);
            if (hours == 0)
                hours += 12;

            int minutes = (int)(hr24 % 1.0 * 60.0);

            return string.IsNullOrWhiteSpace(amPm) ?
                $"{hours}:{minutes:00}" :
                $"{hours}:{minutes:00} {amPm}";
        }

        /// <summary>
        /// Converts the provided time into a HH:MM formatted string using the provided watch level.
        /// </summary>
        /// <returns>The provided time in clock notation.</returns>
        public static string MakeDisplayText(double ticks, bool isDay, WatchLevel watchLevel)
        {
            if (watchLevel == 0)
                return string.Empty;

            double hr24 = To24HourFormat(ticks, isDay);
            string amPm = hr24 < 12.0 ? Language.GetTextValue("GameUI.TimeAtMorning") : Language.GetTextValue("GameUI.TimePastMorning");

            int hours = (int)(hr24 % 12);
            if (hours == 0)
                hours += 12;

            int minutes = watchLevel switch
            {
                WatchLevel.Copper => 0,
                WatchLevel.Silver => (int)(hr24 % 1.0 * 2.0) * 30, // Round down to nearest 30 minutes.
                _ => (int)(hr24 % 1.0 * 60.0),
            };

            return string.IsNullOrWhiteSpace(amPm) ?
                $"{hours}:{minutes:00}" :
                $"{hours}:{minutes:00} {amPm}";
        }

        /// <summary>
        /// Converts the provided time into a HH:MM formatted string using the provided watch level.
        /// </summary>
        /// <returns>The provided time in clock notation.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string MakeDisplayTextForPlayer(double ticks, bool isDay, Player player)
            => MakeDisplayText(ticks, isDay, (WatchLevel)(player?.accWatch ?? 0));

        /// <summary>
        /// Converts the provided time into a HH:MM formatted string.
        /// </summary>
        /// <returns>The provided time in clock notation.</returns>
        public static string MakeDisplayText(double hr24)
        {
            string amPm = hr24 < 12.0 ? Language.GetTextValue("GameUI.TimeAtMorning") : Language.GetTextValue("GameUI.TimePastMorning");

            int hours = (int)(hr24 % 12);
            if (hours == 0)
                hours += 12;

            int minutes = (int)(hr24 % 1.0 * 60.0);

            return string.IsNullOrWhiteSpace(amPm) ?
                $"{hours}:{minutes:00}" :
                $"{hours}:{minutes:00} {amPm}";
        }

        /// <summary>
        /// Converts the provided time into a HH:MM formatted string using the provided watch level.
        /// </summary>
        /// <returns>The provided time in clock notation.</returns>
        public static string MakeDisplayText(double hr24, WatchLevel watchLevel)
        {
            if (watchLevel == 0)
                return string.Empty;

            string amPm = hr24 < 12.0 ? Language.GetTextValue("GameUI.TimeAtMorning") : Language.GetTextValue("GameUI.TimePastMorning");

            int h = (int)(hr24 % 12);
            if (h == 0)
                h += 12;

            int m = watchLevel switch
            {
                WatchLevel.Copper => 0,
                WatchLevel.Silver => (int)(hr24 % 1.0 * 2.0) * 30, // Round down to nearest 30 minutes.
                _ => (int)(hr24 % 1.0 * 60.0),
            };

            return string.IsNullOrWhiteSpace(amPm) ?
                $"{h}:{m:00}" :
                $"{h}:{m:00} {amPm}";
        }

        /// <summary>
        /// Converts the provided time into a HH:MM formatted string using the provided watch level.
        /// </summary>
        /// <returns>The provided time in clock notation.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string MakeDisplayTextForPlayer(double hours, Player player)
            => MakeDisplayText(hours, (WatchLevel)(player?.accWatch ?? 0));

        /// <summary>
        /// Converts the current time into a HH:MM formatted string.
        /// </summary>
        /// <returns>The current time in clock notation.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string MakeCurrentDisplayText()
            => MakeDisplayText(Time24Hours);

        /// <summary>
        /// Converts the current time into a HH:MM formatted string using the provided watch level.
        /// </summary>
        /// <returns>The current time in clock notation.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string MakeCurrentDisplayText(WatchLevel watchLevel)
            => MakeDisplayText(Time24Hours, watchLevel);

        /// <summary>
        /// Converts the current time into a HH:MM formatted string.
        /// </summary>
        /// <returns>The current time in clock notation.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string MakeCurrentDisplayTextForPlayer(Player player)
            => MakeDisplayTextForPlayer(Time24Hours, player);

        /// <summary>
        /// Similar to <see cref="IsDay"/>, but is always false on "dont dig up" worlds.
        /// </summary>
        public static bool IsCertainlyDay => Main.IsItDay();
        /// <summary>
        /// Similar to <see cref="IsNight"/>, but is always false on "dont dig up" worlds.
        /// </summary>
        public static bool IsCertainlyNight => !Main.IsItDay();

        /// <summary>
        /// The current state of the day/night cycle.
        /// <see cref="IsCertainlyDay"/> may be preferred when checking for day time.
        /// </summary>
        public static bool IsDay
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Main.dayTime;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => Main.dayTime = value;
        }
        /// <summary>
        /// The current state of the day/night cycle.
        /// <see cref="IsCertainlyNight"/> may be preferred when checking for night time.
        /// </summary>
        public static bool IsNight
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => !Main.dayTime;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => Main.dayTime = !value;
        }
        /// <summary>
        /// Gets the current time in ticks.
        /// </summary>
        /// <remarks>
        /// This value corresponds to the position of the sun/moon in the sky,
        /// which is to say that it resets to 0 any time the game switches between night and day.
        /// <para/>
        /// See <see cref="DayLengthTicks"/> and <see cref="NightLengthTicks"/> for how long a day/night lasts.
        /// </remarks>
        public static double TimeTicks
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Main.time;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => Main.time = value;
        }

        /// <summary>
        /// The current time in a 24-hour range, where 0.0 is 12:00 AM, 4.5 is 4:30 AM, 13.25 is 1:15 PM, etc.
        /// </summary>
        public static double Time24Hours
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => To24HourFormat(Main.time, Main.dayTime);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => Main.time = ToTickFormat(value, out Main.dayTime);
        }

        /// <summary>
        /// Sets the current time in ticks and runs all the logic for time changes.
        /// For example, starting a random blood moon or goblin invasion.
        /// </summary>
        /// <param name="ticks">The time in ticks to skip to. This parameter will not dictate whether it is day or night.</param>
        /// <param name="isDay">Sets the game to day or night.</param>
        public static void SetTimeTicks(double ticks, bool isDay)
        {
            while (isDay != Main.dayTime)
            {
                bool stopEvents = Main.ShouldNormalEventsBeAbleToStart();
                if (Main.dayTime)
                    Main.UpdateTime_StartNight(ref stopEvents);
                else
                    Main.UpdateTime_StartDay(ref stopEvents);
            }

            Main.time = ticks;
            if (Main.netMode == NetmodeID.Server)
                NetMessage.TrySendData(MessageID.WorldData);
        }

        /// <summary>
        /// Sets the current time in hours and runs all the logic for time changes.
        /// For example, starting a random blood moon or goblin invasion.
        /// </summary>
        /// <param name="hours">The time in hours to skip to.</param>
        public static void SetTimeHours(double hours)
            => SetTimeTicks(ToTickFormat(hours, out bool isDay), isDay);

        /// <summary>
        /// Toggles between day (4:30 AM) and night (7:30 PM) and runs all the logic for time changes.
        /// For example, starting a random blood moon or goblin invasion.
        /// </summary>
        public static void ToggleDayNight()
        {
            bool stopEvents = Main.ShouldNormalEventsBeAbleToStart();
            if (Main.dayTime)
                Main.UpdateTime_StartNight(ref stopEvents);
            else
                Main.UpdateTime_StartDay(ref stopEvents);
        }

        /// <summary>
        /// Skips to dawn (4:30 AM) and runs all the logic for time changes.
        /// For example, starting a random solar eclipse or goblin invasion.
        /// </summary>
        public static void SkipToDay()
        {
            if (!Main.dayTime)
            {
                bool stopEvents = Main.ShouldNormalEventsBeAbleToStart();
                Main.UpdateTime_StartDay(ref stopEvents);
            }
        }


        /// <summary>
        /// Skips to noon (12:00 PM) and runs all the logic for time changes.
        /// For example, starting a random solar eclipse or goblin invasion.
        /// </summary>
        public static void SkipToNoon()
        {
            if (!Main.dayTime)
            {
                bool stopEvents = Main.ShouldNormalEventsBeAbleToStart();
                Main.UpdateTime_StartDay(ref stopEvents);
            }

            Main.time = DayHalfLengthTicks;
        }

        /// <summary>
        /// Skips to dusk (7:30 PM) and runs all the logic for time changes.
        /// For example, starting a random blood moon or boss spawn.
        /// </summary>
        public static void SkipToNight()
        {
            if (Main.dayTime)
            {
                bool stopEvents = Main.ShouldNormalEventsBeAbleToStart();
                Main.UpdateTime_StartNight(ref stopEvents);
            }
        }


        /// <summary>
        /// Skips to midnight (12:00 AM) and runs all the logic for time changes.
        /// For example, starting a random blood moon or boss spawn.
        /// </summary>
        public static void SkipToMidnight()
        {
            if (Main.dayTime)
            {
                bool stopEvents = Main.ShouldNormalEventsBeAbleToStart();
                Main.UpdateTime_StartNight(ref stopEvents);
            }

            Main.time = NightHalfLengthTicks;
        }
    }
}
