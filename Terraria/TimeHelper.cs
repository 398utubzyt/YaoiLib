using System.Runtime.CompilerServices;

using Terraria;
using Terraria.ID;

namespace YaoiLib.Terraria
{
    public delegate void TimeChangeHandler();

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

        public const double TicksToHours = 0.000277777777777777777;
        public const double HoursToTicks = 3600;

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
            get
            {
                if (Main.dayTime)
                    return DayStartHour + Main.time * TicksToHours;

                if (Main.time < NightHalfLengthTicks)
                    return NightStartHour + Main.time * TicksToHours;

                return Main.time * TicksToHours;
            }
            set
            {
                value %= 24.0;
                if (value < 0.0)
                    value += 24.0;

                if (value < DayStartHour)
                {
                    Main.dayTime = false;
                    Main.time = NightHalfLengthTicks + (value * HoursToTicks);
                } else if (value < NightStartHour)
                {
                    Main.dayTime = true;
                    Main.time = (value - DayStartHour) * HoursToTicks;
                } else
                {
                    Main.dayTime = false;
                    Main.time = (value - NightStartHour) * HoursToTicks;
                }
            }
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
        {
            hours %= 24.0;
            if (hours < 0.0)
                hours += 24.0;

            double ticks;
            bool isDay;

            if (hours < DayStartHour)
            {
                ticks = NightHalfLengthTicks + (hours * HoursToTicks);
                isDay = false;
            } else if (hours < NightStartHour)
            {
                ticks = (hours - DayStartHour) * HoursToTicks;
                isDay = true;
            } else
            {
                ticks = (hours - NightStartHour) * HoursToTicks;
                isDay = false;
            }

            SetTimeTicks(ticks, isDay);
        }

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
