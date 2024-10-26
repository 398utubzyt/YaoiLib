using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;

using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace YaoiLib.Terraria
{
    /// <summary>
    /// Represents a music track in Terraria.
    /// </summary>
    public ref struct MusicInfo
    {
        public int Id;
        public ref float MixVolume;
        public ref bool ShouldNotFade;
        public ref IAudioTrack AudioTrack;

        /// <summary>
        /// Gets if the track is currently valid.
        /// </summary>
        public readonly bool IsValid
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => MusicHelper.IsValidMusicId(Id);
        }
        /// <summary>
        /// Gets if the track is a valid modded track.
        /// </summary>
        public readonly bool IsModded
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => MusicHelper.IsModdedMusicId(Id);
        }
        /// <summary>
        /// Gets if the track is a valid vanilla track.
        /// </summary>
        public readonly bool IsVanilla
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => MusicHelper.IsVanillaMusicId(Id);
        }

        /// <summary>
        /// Gets if the track is the current track.
        /// </summary>
        public readonly bool IsCurrentMusic
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => MusicHelper.IsValidMusicId(Id) && MusicHelper.CurrentMusicId == Id;
        }
        /// <summary>
        /// Gets if the track is queued to play next.
        /// </summary>
        public readonly bool IsQueuedMusic
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => MusicHelper.IsValidMusicId(Id) && MusicHelper.QueuedMusicId == Id;
        }

        /// <summary>
        /// Gets if the track is actively playing.
        /// </summary>
        public readonly bool IsPlaying
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => AudioTrack?.IsPlaying ?? false;
        }
        /// <summary>
        /// Gets if the track is stopped.
        /// </summary>
        public readonly bool IsStopped
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => AudioTrack?.IsStopped ?? false;
        }
        /// <summary>
        /// Gets if the track is paused.
        /// </summary>
        public readonly bool IsPaused
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => AudioTrack?.IsPaused ?? false;
        }

        /// <summary>
        /// Tries to get the path of the modded track.
        /// </summary>
        /// <param name="path">The path of the music track.</param>
        /// <returns>
        /// <see langword="true"/> if the track is modded and the path was returned successfully;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool TryGetPath([MaybeNullWhen(false)] out string path)
            => MusicHelper.TryGetMusicPath(Id, out path);

        /// <summary>
        /// Tries to get the name of the mod which this track belongs to.
        /// </summary>
        /// <param name="name">The name of the mod which owns this track.</param>
        /// <returns>
        /// <see langword="true"/> if the track is modded and the mod name was returned successfully;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        public readonly bool TryGetModName([MaybeNullWhen(false)] out string name)
        {
            bool result = TryGetPath(out string path);

            name = result ? path[..path.IndexOf('/')] : null;

            return result;
        }

        /// <summary>
        /// Tries to get the mod which this track belongs to.
        /// </summary>
        /// <param name="mod">The mod which owns this track.</param>
        /// <returns>
        /// <see langword="true"/> if the track is modded and the mod was returned successfully;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        public readonly bool TryGetMod([MaybeNullWhen(false)] out Mod mod)
        {
            bool result = TryGetModName(out string name);

            if (result)
            {
                result = ModLoader.TryGetMod(name, out mod);
            } else
            {
                mod = null;
            }

            return result;
        }

        /// <summary>
        /// Attempts to queue this track to play next.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if the track was enqueued successfully;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool TryEnqueue()
            => MusicHelper.TryQueueMusic(Id);
    }

    /// <summary>
    /// Provides utility functions and properties for the background music.
    /// </summary>
    public static class MusicHelper
    {
        /// <summary>
        /// ID of no music. Typically used as the "invalid" ID.
        /// </summary>
        public const int Silence = 0;
        /// <summary>
        /// The theoretical first ID of the modded music.
        /// </summary>
        public const int ModdedStart = MusicID.Count;

        private static Dictionary<string, int> _mlByPath;

        private static float _nilMV;
        private static bool _nilDF;
        private static IAudioTrack _nilAT;

        internal static void Load()
        {
            _mlByPath = typeof(MusicLoader).GetField("musicByPath")?.GetValue(null) as Dictionary<string, int>;
        }

        internal static void Unload()
        {
        }

        /// <summary>
        /// The "mixer" of Terraria. Controls the fader volumes of every track in the game.
        /// </summary>
        public static float[] MixerVolumes
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Main.musicFade;
        }

        /// <summary>
        /// Gets if the game is using a valid audio system.
        /// </summary>
        public static bool IsAudioSystemEnabled
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Main.audioSystem is LegacyAudioSystem;
        }

        /// <summary>
        /// Gets the current audio system if it is valid; otherwise, <see langword="null"/>.
        /// </summary>
        public static LegacyAudioSystem AudioSystem
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Main.audioSystem as LegacyAudioSystem;
        }

        /// <summary>
        /// The ID of the current track.
        /// </summary>
        public static int CurrentMusicId
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Main.curMusic;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => Main.curMusic = value;
        }

        /// <summary>
        /// Gets the <see cref="IAudioTrack"/> of the current track.
        /// </summary>
        public static IAudioTrack CurrentMusicTrack
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => AudioSystem.AudioTracks[Main.curMusic];
        }

        /// <summary>
        /// Gets the info of the current track.
        /// </summary>
        public static MusicInfo CurrentMusicInfo
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                TryGetMusicInfo(Main.curMusic, out MusicInfo info);
                return info;
            }
        }

        /// <summary>
        /// Gets if the current track is valid.
        /// </summary>
        public static bool IsCurrentMusicValid
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => IsValidMusicId(Main.curMusic);
        }

        /// <summary>
        /// Gets if the current track is valid and modded.
        /// </summary>
        public static bool IsCurrentMusicModded
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => IsModdedMusicId(Main.curMusic);
        }

        /// <summary>
        /// Gets if the current track is valid and vanilla.
        /// </summary>
        public static bool IsCurrentMusicVanilla
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => IsVanillaMusicId(Main.curMusic);
        }

        /// <summary>
        /// The mix volume of the current track.
        /// </summary>
        public static float CurrentMixVolume
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Main.musicFade[Main.curMusic];
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => Main.musicFade[Main.curMusic] = value;
        }

        /// <summary>
        /// The ID of the queued track.
        /// </summary>
        public static int QueuedMusicId
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Main.newMusic;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => Main.newMusic = value;
        }

        /// <summary>
        /// Gets the <see cref="IAudioTrack"/> of the queued track.
        /// </summary>
        public static IAudioTrack QueuedMusicTrack
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => AudioSystem.AudioTracks[Main.newMusic];
        }

        /// <summary>
        /// Gets the info of the queued track.
        /// </summary>
        public static MusicInfo QueuedMusicInfo
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                TryGetMusicInfo(Main.newMusic, out MusicInfo info);
                return info;
            }
        }

        /// <summary>
        /// Gets if the queued track is valid.
        /// </summary>
        public static bool HasQueuedMusic
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => IsValidMusicId(Main.newMusic);
        }

        /// <summary>
        /// Gets if the queued track is valid and modded.
        /// </summary>
        public static bool IsQueuedMusicModded
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => IsModdedMusicId(Main.newMusic);
        }

        /// <summary>
        /// Gets if the queued track is valid and vanilla.
        /// </summary>
        public static bool IsQueuedMusicVanilla
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => IsVanillaMusicId(Main.newMusic);
        }

        /// <summary>
        /// The mix volume of the queued track.
        /// </summary>
        public static float QueuedMixVolume
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Main.musicFade[Main.newMusic];
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => Main.musicFade[Main.newMusic] = value;
        }

        /// <summary>
        /// Gets if the provided music ID is valid.
        /// </summary>
        /// <param name="id">The ID to check.</param>
        /// <returns><see langword="true"/> if the ID is valid; otherwise, <see langword="false"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValidMusicId(int id)
        {
            return id > Silence && id < MusicLoader.MusicCount;
        }

        /// <summary>
        /// Gets if the provided music ID is valid and belongs to a modded track.
        /// </summary>
        /// <param name="id">The ID to check.</param>
        /// <returns>
        /// <see langword="true"/> if the ID is valid and belongs to a modded track;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsModdedMusicId(int id)
        {
            return id >= ModdedStart && id < MusicLoader.MusicCount;
        }

        /// <summary>
        /// Gets if the provided music ID is valid and belongs to a vanilla track.
        /// </summary>
        /// <param name="id">The ID to check.</param>
        /// <returns>
        /// <see langword="true"/> if the ID is valid and belongs to a vanilla track;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsVanillaMusicId(int id)
        {
            return id > Silence && id < ModdedStart;
        }

        /// <summary>
        /// Attempts to query track info from the music ID.
        /// </summary>
        /// <param name="id">The ID to query.</param>
        /// <param name="info">The corresponding music info.</param>
        /// <returns>
        /// <see langword="true"/> if the ID is valid and the query was successful;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        public static bool TryGetMusicInfo(int id, out MusicInfo info)
        {
            bool result = IsValidMusicId(id);

            if (result)
            {
                info.Id = id;
                info.MixVolume = ref Main.musicFade[id];
                info.ShouldNotFade = ref Main.musicNoCrossFade[id];
                info.AudioTrack = ref AudioSystem.AudioTracks[id];
            } else
            {
                info.Id = 0;
                info.MixVolume = ref _nilMV; // Don't return null ref
                info.ShouldNotFade = ref _nilDF; // Don't return null ref
                info.AudioTrack = ref _nilAT; // Don't return null ref
            }

            return result;
        }

        /// <summary>
        /// Attempts to query track info from the music path.
        /// </summary>
        /// <param name="path">The path of the music to query.</param>
        /// <param name="info">The corresponding music info.</param>
        /// <returns>
        /// <see langword="true"/> if the path is valid and the query was successful;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        public static bool TryGetMusicInfo(string path, out MusicInfo info)
        {
            return TryGetMusicInfo(MusicLoader.GetMusicSlot(path), out info);
        }

        /// <summary>
        /// Attempts to query track info from the music path and the mod which it belongs to.
        /// </summary>
        /// <param name="mod">The mod which the music belongs to.</param>
        /// <param name="path">The path of the music to query.</param>
        /// <param name="info">The corresponding music info.</param>
        /// <returns>
        /// <see langword="true"/> if the mod and path are valid and the query was successful;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        public static bool TryGetMusicInfo(Mod mod, string path, out MusicInfo info)
        {
            return TryGetMusicInfo(MusicLoader.GetMusicSlot(mod, path), out info);
        }

        /// <summary>
        /// Checks if the music ID belongs to the provided mod.
        /// </summary>
        /// <param name="id">The ID to check.</param>
        /// <param name="mod">The mod which the music belongs to.</param>
        /// <returns>
        /// <see langword="true"/> if the ID is valid and belongs to the provided mod;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        public static bool MusicBelongsTo(int id, Mod mod)
        {
            bool result = TryGetMusicPath(id, out string path);
            
            if (result)
            {
                result = path.Length > mod.Name.Length &&
                    path.StartsWith(mod.Name, StringComparison.Ordinal) &&
                    path[mod.Name.Length] == '/';
            }

            return result;
        }

        /// <summary>
        /// Attempts to get the mix volume of the track with the corresponding ID.
        /// </summary>
        /// <param name="id">The ID of the track to get the volume of.</param>
        /// <param name="volume">The mix volume of the track.</param>
        /// <returns>
        /// <see langword="true"/> if the ID is valid and the volume was returned successfully;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        public static bool TryGetMusicMixVolume(int id, out float volume)
        {
            bool result = IsValidMusicId(id);

            volume = result ? Main.musicFade[id] : 0.0f;

            return result;
        }

        /// <summary>
        /// Attempts to set the mix volume of the track with the corresponding ID.
        /// </summary>
        /// <param name="id">The ID of the track to set the volume of.</param>
        /// <param name="volume">The new mix volume of the track.</param>
        /// <returns>
        /// <see langword="true"/> if the ID is valid and the volume was set successfully;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        public static bool TrySetMusicMixVolume(int id, float volume)
        {
            bool result = IsValidMusicId(id);

            if (result)
            {
                Main.musicFade[id] = volume;
            }

            return result;
        }

        /// <summary>
        /// Attempts to get the modded path of the music using its ID.
        /// </summary>
        /// <param name="id">The ID of the modded music to get the path of.</param>
        /// <param name="path">The path of the music.</param>
        /// <returns>
        /// <see langword="true"/> if the ID is valid and modded, and the path was returned successfully;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        public static bool TryGetMusicPath(int id, [MaybeNullWhen(false)] out string path)
        {
            bool result = IsModdedMusicId(id);

            if (result)
            {
                path = _mlByPath.Keys.FirstOrDefault(s => _mlByPath[s] == id);
            } else
            {
                path = null;
            }

            return result;
        }

        /// <summary>
        /// Queues nothing.
        /// </summary>
        public static void QueueSilence()
        {
            Main.newMusic = Silence;
        }

        /// <summary>
        /// Puts music with the provided ID into the queue.
        /// </summary>
        /// <param name="id">The ID of the music to enqueue.</param>
        /// <returns>
        /// <see langword="true"/> if the ID is valid and the music was enqueued;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        public static bool TryQueueMusic(int id)
        {
            bool result = IsValidMusicId(id);
            
            if (result)
            {
                Main.newMusic = id;
            }

            return result;
        }

        /// <summary>
        /// Puts music with the provided path into the queue.
        /// </summary>
        /// <param name="path">The path of the music to enqueue.</param>
        /// <returns>
        /// <see langword="true"/> if the path is valid and the music was enqueued;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        public static bool TryQueueMusic(string path)
        {
            return TryQueueMusic(MusicLoader.GetMusicSlot(path));
        }

        /// <summary>
        /// Puts music with the provided path and mod into the queue.
        /// </summary>
        /// <param name="mod">The mod of the music to enqueue.</param>
        /// <param name="path">The path of the music to enqueue.</param>
        /// <returns>
        /// <see langword="true"/> if the path and mod are valid and the music was enqueued;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        public static bool TryQueueMusic(Mod mod, string path)
        {
            return TryQueueMusic(MusicLoader.GetMusicSlot(mod, path));
        }

        public static bool CalculateFirstMusicSlot(Mod mod, out int slot)
        {
            slot = Silence;

            for (int i = ModdedStart; i < MusicLoader.MusicCount; ++i)
            {
                if (MusicBelongsTo(i, mod))
                {
                    slot = i;
                    break;
                }
            }

            if (slot == Silence)
            {
                if (mod.IsLoading())
                {
                    slot = MusicLoader.MusicCount;
                }
            }

            return slot != Silence;
        }
    }
}
