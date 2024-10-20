using System.Runtime.CompilerServices;

using Terraria;

namespace YaoiLib.Terraria
{
    /// <summary>
    /// Provides utility functions and properties for the rain.
    /// </summary>
    public static class RainHelper
    {
        /// <summary>
        /// Gets if it is currently raining.
        /// As opposed to <see cref="IsRaining"/>, this returns the current state of the rain.
        /// </summary>
        public static bool IsCertainlyRaining => Main.IsItRaining;
        /// <summary>
        /// Gets if it is currently storming.
        /// </summary>
        public static bool IsCertainlyStormy => Main.IsItStorming;

        /// <summary>
        /// The target state of the rain.
        /// When set, the game will attempt to transition to the corresponding state.
        /// </summary>
        public static bool IsRaining
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Main.raining;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => Main.raining = value;
        }
        /// <summary>
        /// The time which the rain will remain active for.
        /// </summary>
        public static double RainTicksRemaining
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Main.rainTime;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => Main.rainTime = value;
        }
        /// <summary>
        /// The target rain intensity.
        /// When <see cref="IsRaining"/> is <see langword="true"/>, the game will attempt to transition to this intensity.
        /// </summary>
        public static float TargetRainIntensity
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Main.maxRaining;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => Main.maxRaining = value;
        }
        /// <summary>
        /// The current intensity of the rain.
        /// Typically approaches <see cref="TargetRainIntensity"/>.
        /// </summary>
        public static float RainIntensity
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Main.cloudAlpha;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => Main.cloudAlpha = value;
        }

        /// <summary>
        /// Sets both <see cref="TargetRainIntensity"/> and <see cref="RainIntensity"/>
        /// simultaneously to avoid transitioning into the new intensity.
        /// </summary>
        /// <param name="intensity">The intensity of the rain between 0 and 1.</param>
        public static void SetRainIntensity(float intensity)
        {
            Main.maxRaining = intensity;
            Main.cloudAlpha = intensity;
        }

        /// <summary>
        /// Starts naturally raining just like it does in vanilla.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void StartNaturalRain()
        {
            Main.StartRain();
        }

        /// <summary>
        /// Starts raining with the provided length and target intensity.
        /// </summary>
        /// <param name="duration">The total duration of the rain in ticks.</param>
        /// <param name="intensity">The intensity of the rain between 0 and 1.</param>
        public static void StartRain(double duration, float intensity)
        {
            Main.rainTime = duration;
            Main.maxRaining = intensity;
            Main.raining = true;
        }

        /// <summary>
        /// Sets the rain to the provided length and intensity.
        /// </summary>
        /// <param name="duration">The total duration of the rain in ticks.</param>
        /// <param name="intensity">The intensity of the rain between 0 and 1.</param>
        public static void SetRain(double duration, float intensity)
        {
            Main.rainTime = duration;
            Main.maxRaining = intensity;
            Main.cloudAlpha = intensity;
            Main.raining = true;
        }

        /// <summary>
        /// Stops the rain.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void StopRain()
        {
            Main.StopRain();
        }

        /// <summary>
        /// Stops the rain instantly without the visual transition.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ClearRain()
        {
            Main.StopRain();
            Main.cloudAlpha = 0.0f;
        }
    }
}
