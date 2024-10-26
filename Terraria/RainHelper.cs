using System.Runtime.CompilerServices;

using Terraria;

namespace YaoiLib.Terraria
{
    public delegate void WeatherChangeHandler();

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

        internal static void Load()
        {
            On_Main.StartRain += On_StartRain;
            On_Main.StopRain += On_StopRain;
            On_Main.UpdateWindyDayState += On_UpdateWindyDayState;
        }

        internal static void Unload()
        {
            _onStartRaining = null;
            _onStartStorming = null;
            _onStopRaining = null;
            _onStopStorming = null;
        }

        private static void On_StartRain(On_Main.orig_StartRain orig)
        {
            orig();

            if (Main.raining)
                _onStartRaining?.Invoke();
        }

        private static void On_StopRain(On_Main.orig_StopRain orig)
        {
            orig();

            if (!Main.raining)
                _onStopRaining?.Invoke();
        }

        private static void On_UpdateWindyDayState(On_Main.orig_UpdateWindyDayState orig, Main self)
        {
            bool isStorming = Main.IsItStorming;

            orig(self);
            
            bool isStormingNow = Main.IsItStorming;

            if (isStorming != isStormingNow)
            {
                if (isStormingNow)
                    _onStartStorming?.Invoke();
                else
                    _onStopStorming?.Invoke();
            }
        }

        private static event WeatherChangeHandler _onStartRaining;
        private static event WeatherChangeHandler _onStartStorming;
        private static event WeatherChangeHandler _onStopRaining;
        private static event WeatherChangeHandler _onStopStorming;

        /// <summary>
        /// Invoked after it starts raining.
        /// </summary>
        public static event WeatherChangeHandler OnStartRaining
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            add => _onStartRaining += value;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            remove => _onStartRaining -= value;
        }

        /// <summary>
        /// Invoked after it starts storming.
        /// </summary>
        public static event WeatherChangeHandler OnStartStorming
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            add => _onStartStorming += value;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            remove => _onStartStorming -= value;
        }

        /// <summary>
        /// Invoked after it stops raining.
        /// </summary>
        public static event WeatherChangeHandler OnStopRaining
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            add => _onStopRaining += value;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            remove => _onStopRaining -= value;
        }

        /// <summary>
        /// Invoked after it stops storming.
        /// </summary>
        public static event WeatherChangeHandler OnStopStorming
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            add => _onStopStorming += value;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            remove => _onStopStorming -= value;
        }

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
