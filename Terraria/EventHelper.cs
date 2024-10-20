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

    /// <summary>
    /// Provides utility functions and properties for vanilla events.
    /// </summary>
    public static class EventHelper
    {
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
