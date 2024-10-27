using System;
using System.Runtime.CompilerServices;

using Terraria;
using Terraria.GameContent.Events;
using Terraria.ID;

namespace YaoiLib.Terraria
{
    /// <summary>
    /// Provides utility functions and properties for the wind.
    /// </summary>
    public static class WindHelper
    {
        /// <summary>
        /// Gets if it is currently a windy day event.
        /// </summary>
        public static bool IsWindyDay => Main.IsItAHappyWindyDay;

        /// <summary>
        /// The current speed of the wind from <c>-0.8</c> to <c>0.8</c>.
        /// Typically approaches <see cref="TargetWindSpeed"/>.
        /// </summary>
        public static float WindSpeed
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Main.windSpeedCurrent;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => Main.windSpeedCurrent = value;
        }

        /// <summary>
        /// The current direction of the wind.
        /// </summary>
        public static float WindDirection
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Math.Sign(Main.windSpeedCurrent);
        }

        /// <summary>
        /// The target speed of the wind from <c>-0.8</c> to <c>0.8</c>.
        /// When set, the game will attempt to transition to this wind speed.
        /// </summary>
        public static float TargetWindSpeed
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Main.windSpeedTarget;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => Main.windSpeedTarget = value;
        }

        /// <summary>
        /// Gets if the current wind speed is sustainable for a sandstorm.
        /// </summary>
        public static bool SustainableForSandstorm
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Math.Abs(Main.windSpeedCurrent) >= 0.6f;
        }

        /// <summary>
        /// Gets the projected sandstorm lifetime, taking wind speed into consideration.
        /// </summary>
        public static double ProjectedSandstormLifetime
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                double timeLeft = Sandstorm.TimeLeft / TimeHelper.EventUpdateRate;
                if (Math.Abs(Main.windSpeedCurrent) < 0.6f)
                {
                    timeLeft /= 16.0;
                }
                return timeLeft;
            }
        }

        /// <summary>
        /// Sets the current and target wind speed simultaneously.
        /// </summary>
        /// <param name="speed">The speed to set.</param>
        public static void SetWindSpeed(float speed)
        {
            Main.windSpeedTarget = speed;
            Main.windSpeedCurrent = speed;
            NetMessage.SendData(MessageID.WorldData);
        }
    }
}
