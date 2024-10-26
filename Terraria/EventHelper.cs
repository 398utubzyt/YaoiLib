using System.Runtime.CompilerServices;

using Terraria;
using Terraria.GameContent.Achievements;
using Terraria.ID;

namespace YaoiLib.Terraria
{
    public enum VanillaEventID
    {
        None,
        BloodMoon,
        PumpkinMoon,
        FrostMoon,
        SolarEclipse,
    }

    public delegate void EventChangeHandler();

    /// <summary>
    /// Provides utility functions and properties for vanilla events.
    /// </summary>
    public static class EventHelper
    {
        internal static void Load()
        {
            On_Main.UpdateTime_StartDay += On_UpdateTime_StartDay;
            On_Main.UpdateTime_StartNight += On_UpdateTime_StartNight;

            On_Main.startPumpkinMoon += On_startPumpkinMoon;
            On_Main.startSnowMoon += On_startSnowMoon;
            On_Main.stopMoonEvent += On_stopMoonEvent;
        }

        internal static void Unload()
        {
            _onBloodMoonStart = null;
            _onBloodMoonStop = null;
            _onPumpkinMoonStart = null;
            _onPumpkinMoonStop = null;
            _onFrostMoonStart = null;
            _onFrostMoonStop = null;
            _onSolarEclipseStart = null;
            _onSolarEclipseStop = null;
        }

        private static void On_UpdateTime_StartDay(On_Main.orig_UpdateTime_StartDay orig, ref bool stopEvents)
        {
            bool wasBloodMoon = Main.bloodMoon;

            orig(ref stopEvents);

            if (wasBloodMoon)
                _onBloodMoonStop?.Invoke();

            // Pumpkin/frost moons get handled in stopMoonEvent()

            if (Main.eclipse)
                _onSolarEclipseStart?.Invoke();
        }

        private static void On_UpdateTime_StartNight(On_Main.orig_UpdateTime_StartNight orig, ref bool stopEvents)
        {
            bool wasEclipse = Main.eclipse;

            orig(ref stopEvents);

            if (wasEclipse)
                _onSolarEclipseStop?.Invoke();

            if (Main.bloodMoon)
                _onBloodMoonStart?.Invoke();
        }

        private static void On_startPumpkinMoon(On_Main.orig_startPumpkinMoon orig)
        {
            orig();
            if (Main.pumpkinMoon)
                _onPumpkinMoonStart?.Invoke();
        }

        private static void On_startSnowMoon(On_Main.orig_startSnowMoon orig)
        {
            orig();
            if (Main.snowMoon)
                _onFrostMoonStart?.Invoke();
        }

        private static void On_stopMoonEvent(On_Main.orig_stopMoonEvent orig)
        {
            bool wasPumpkin = Main.pumpkinMoon;
            bool wasFrost = Main.snowMoon;

            orig();

            if (wasPumpkin && !Main.pumpkinMoon)
                _onPumpkinMoonStop?.Invoke();
            if (wasFrost && !Main.snowMoon)
                _onFrostMoonStop?.Invoke();
        }

        private static event EventChangeHandler _onBloodMoonStart;
        private static event EventChangeHandler _onBloodMoonStop;
        private static event EventChangeHandler _onPumpkinMoonStart;
        private static event EventChangeHandler _onPumpkinMoonStop;
        private static event EventChangeHandler _onFrostMoonStart;
        private static event EventChangeHandler _onFrostMoonStop;
        private static event EventChangeHandler _onSolarEclipseStart;
        private static event EventChangeHandler _onSolarEclipseStop;

        /// <summary>
        /// Invoked after a blood moon event starts.
        /// </summary>
        public static event EventChangeHandler OnBloodMoonStart
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            add => _onBloodMoonStart += value;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            remove => _onBloodMoonStart -= value;
        }

        /// <summary>
        /// Invoked after a blood moon event stops.
        /// </summary>
        public static event EventChangeHandler OnBloodMoonStop
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            add => _onBloodMoonStop += value;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            remove => _onBloodMoonStop -= value;
        }

        /// <summary>
        /// Invoked after a pumpkin moon event starts.
        /// </summary>
        public static event EventChangeHandler OnPumpkinMoonStart
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            add => _onPumpkinMoonStart += value;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            remove => _onPumpkinMoonStart -= value;
        }

        /// <summary>
        /// Invoked after a pumpkin moon event stops.
        /// </summary>
        public static event EventChangeHandler OnPumpkinMoonStop
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            add => _onPumpkinMoonStop += value;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            remove => _onPumpkinMoonStop -= value;
        }

        /// <summary>
        /// Invoked after a frost moon event stops.
        /// </summary>
        public static event EventChangeHandler OnFrostMoonStart
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            add => _onFrostMoonStart += value;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            remove => _onFrostMoonStart -= value;
        }

        /// <summary>
        /// Invoked after a frost moon event stops.
        /// </summary>
        public static event EventChangeHandler OnFrostMoonStop
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            add => _onFrostMoonStop += value;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            remove => _onFrostMoonStop -= value;
        }

        /// <summary>
        /// Invoked after a solar eclipse event starts.
        /// </summary>
        public static event EventChangeHandler OnSolarEclipseStart
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            add => _onSolarEclipseStart += value;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            remove => _onSolarEclipseStart -= value;
        }

        /// <summary>
        /// Invoked after a solar eclipse event stops.
        /// </summary>
        public static event EventChangeHandler OnSolarEclipseStop
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            add => _onSolarEclipseStop += value;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            remove => _onSolarEclipseStop -= value;
        }

        /// <summary>
        /// The state of the blood moon event.
        /// </summary>
        public static bool IsBloodMoon
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Main.bloodMoon;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => Main.bloodMoon = value;
        }
        /// <summary>
        /// The state of the pumpkin moon event.
        /// </summary>
        public static bool IsPumpkinMoon
        {
            get => Main.pumpkinMoon;
            set => Main.pumpkinMoon = value;
        }
        /// <summary>
        /// The state of the frost moon event.
        /// </summary>
        public static bool IsFrostMoon
        {
            get => Main.snowMoon;
            set => Main.snowMoon = value;
        }
        /// <summary>
        /// The state of the solar eclipse event.
        /// </summary>
        public static bool IsSolarEclipse
        {
            get => Main.eclipse;
            set => Main.eclipse = value;
        }

        /// <summary>
        /// The current active event.
        /// </summary>
        public static VanillaEventID CurrentEvent
        {
            get
            {
                if (TimeHelper.IsCertainlyDay)
                {
                    if (Main.eclipse)
                        return VanillaEventID.SolarEclipse;
                } else
                {
                    if (Main.pumpkinMoon)
                        return VanillaEventID.PumpkinMoon;
                    if (Main.snowMoon)
                        return VanillaEventID.FrostMoon;
                    if (Main.bloodMoon)
                        return VanillaEventID.BloodMoon;
                }

                return VanillaEventID.SolarEclipse;
            }
        }

        /// <summary>
        /// Starts an event with the corresponding <see cref="VanillaEventID"/>.
        /// </summary>
        /// <param name="id">The event to start.</param>
        /// <returns><see langword="true"/> if an event was started, otherwise <see langword="false"/>.</returns>
        public static bool StartEvent(VanillaEventID id)
        {
            return id switch
            {
                VanillaEventID.BloodMoon => StartBloodMoon(),
                VanillaEventID.PumpkinMoon => StartPumpkinMoon(),
                VanillaEventID.FrostMoon => StartFrostMoon(),
                VanillaEventID.SolarEclipse => StartSolarEclipse(),
                _ => false,
            };
        }

        /// <summary>
        /// Stops all active vanilla events.
        /// </summary>
        public static void StopAllEvents()
        {
            Main.bloodMoon = false;
            Main.stopMoonEvent();
            Main.eclipse = false;
            NetHelper.SyncWorldData();
        }


        /// <summary>
        /// If <see cref="TimeHelper.IsCertainlyNight"/> is <see langword="true"/>,
        /// this starts a blood moon event as if the event were naturally occuring.
        /// </summary>
        /// <returns><see langword="true"/> if a blood moon event was started, otherwise <see langword="false"/>.</returns>
        public static bool StartBloodMoon()
        {
            if (TimeHelper.IsCertainlyDay || Main.pumpkinMoon || Main.snowMoon)
                return false;

            Main.bloodMoon = true;

            Main.sundialCooldown = 0;
            Main.moondialCooldown = 0;
            AchievementsHelper.NotifyProgressionEvent(AchievementHelperID.Events.BloodMoonStart);

            NetHelper.ChatPost(Lang.misc[8], 50, 255, 130);
            return true;
        }

        /// <summary>
        /// If <see cref="TimeHelper.IsCertainlyNight"/> is <see langword="true"/>,
        /// this starts a pumpkin moon event as if a pumpkin medallion was used.
        /// </summary>
        /// <returns><see langword="true"/> if a pumpkin moon event was started, otherwise <see langword="false"/>.</returns>
        public static bool StartPumpkinMoon()
        {
            if (TimeHelper.IsCertainlyDay)
                return false;

            Main.startPumpkinMoon();
            NetHelper.SyncWorldData();
            return true;
        }

        /// <summary>
        /// If <see cref="TimeHelper.IsCertainlyNight"/> is <see langword="true"/>,
        /// this starts a frost moon event as if a naughty present was used.
        /// </summary>
        /// <returns><see langword="true"/> if a frost moon event was started, otherwise <see langword="false"/>.</returns>
        public static bool StartFrostMoon()
        {
            if (TimeHelper.IsCertainlyDay)
                return false;

            Main.startSnowMoon();
            NetHelper.SyncWorldData();
            return true;
        }

        /// <summary>
        /// If <see cref="TimeHelper.IsCertainlyDay"/> is <see langword="true"/>,
        /// this starts a solar eclipse event as if the event were naturally occuring.
        /// </summary>
        /// <returns><see langword="true"/> if a solar eclipse event was started, otherwise <see langword="false"/>.</returns>
        public static bool StartSolarEclipse()
        {
            if (TimeHelper.IsCertainlyNight)
                return false;

            AchievementsHelper.NotifyProgressionEvent(AchievementHelperID.Events.EclipseStart);

            if (Main.remixWorld)
            {
                NetHelper.ChatPost(Lang.misc[106], 50, 255, 130);
            } else
            {
                NetHelper.ChatPost(Lang.misc[20], 50, 255, 130);
            }

            NetHelper.SyncWorldData();
            return true;
        }

        /// <summary>
        /// Stops the current blood moon event.
        /// </summary>
        public static void StopBloodMoon()
        {
            Main.bloodMoon = false;
            NetHelper.SyncWorldData();
        }

        /// <summary>
        /// Stops the current pumpkin moon event.
        /// </summary>
        public static void StopPumpkinMoon()
        {
            Main.stopMoonEvent();
            NetHelper.SyncWorldData();
        }

        /// <summary>
        /// Stops the current frost moon event.
        /// </summary>
        public static void StopFrostMoon()
        {
            Main.stopMoonEvent();
            NetHelper.SyncWorldData();
        }

        /// <summary>
        /// Stops the current solar eclipse event.
        /// </summary>
        public static void StopSolarEclipse()
        {
            Main.eclipse = false;
            NetHelper.SyncWorldData();
        }
    }
}
