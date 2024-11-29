using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace YaoiLib.Terraria
{
    public readonly ref struct BossIterator(ReadOnlySpan<NPC> npcs)
    {
        private readonly ReadOnlySpan<NPC> _npcs = npcs;

        public Enumerator GetEnumerator() => new(_npcs.GetEnumerator());
        public EowEnumerator GetEowEnumerator() => new(_npcs.GetEnumerator());

        public ref struct Enumerator(ReadOnlySpan<NPC>.Enumerator enumerator)
        {
            private ReadOnlySpan<NPC>.Enumerator _enumerator = enumerator;
            private bool _eow;

            public readonly NPC Current => _enumerator.Current;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool MoveNext()
            {
                while (_enumerator.MoveNext())
                {
                    if (_enumerator.Current.active)
                    {
                        if (NpcHelper.ShouldCountAsBoss(_enumerator.Current))
                            return true;
                    }
                }

                return false;
            }
        }

        public ref struct EowEnumerator(ReadOnlySpan<NPC>.Enumerator enumerator)
        {
            private ReadOnlySpan<NPC>.Enumerator _enumerator = enumerator;

            public readonly NPC Current => _enumerator.Current;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool MoveNext()
            {
                while (_enumerator.MoveNext())
                {
                    if (_enumerator.Current.active && NpcHelper.IsEowSegment(_enumerator.Current))
                        return true;
                }

                return false;
            }
        }
    }

    public delegate void NpcSpawnHandler(NPC npc, IEntitySource source);
    public delegate void NpcKillHandler(NPC npc);

    /// <summary>
    /// Provides utility functions and properties for NPCs.
    /// </summary>
    public static class NpcHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsEowSegment(NPC npc)
        {
            return npc.type >= NPCID.EaterofWorldsHead && npc.type <= NPCID.EaterofWorldsTail;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int EowSegmentCount()
        {
            int c = 0;
            BossIterator.EowEnumerator e = ActiveBosses.GetEowEnumerator();
            while (e.MoveNext())
                c++;
            return c;
        }

        private class NpcHelperGlobalNPC : GlobalNPC
        {
            public override void OnSpawn(NPC npc, IEntitySource source)
            {
                if (ShouldCountAsBoss(npc))
                {
                    _onBossSpawn?.Invoke(npc, source);
                    _npcWasAlive[npc.whoAmI] = true;
                    UpdateCache();
                }
            }

            public override void OnKill(NPC npc)
            {
                if (!NetHelper.IsClient)
                {
                    if (ShouldCountAsBoss(npc))
                    {
                        _onBossDie?.Invoke(npc);
                        _npcWasAlive[npc.whoAmI] = false;
                        UpdateCacheExcept(npc);
                    }
                }
            }

            public override void HitEffect(NPC npc, NPC.HitInfo hit)
            {
                if (NetHelper.IsClient)
                {
                    if (ShouldCountAsBoss(npc) && npc.life <= 0)
                    {
                        _onBossDie?.Invoke(npc);
                        _npcWasAlive[npc.whoAmI] = false;
                        UpdateCacheExcept(npc);
                    }
                }
            }

            public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
            {
                return lateInstantiation && entity.boss;
            }
        }

        private class NpcHelperModSystem : ModSystem
        {
            public override void PostUpdateNPCs()
            {
                ref bool wasAlive = ref MemoryMarshal.GetArrayDataReference(_npcWasAlive);
                foreach (NPC npc in Main.npc)
                {
                    if (!npc.active)
                    {
                        if (wasAlive)
                        {
                            _onBossDespawn?.Invoke(npc);
                            wasAlive = false;
                            UpdateCache();
                        }
                    }

                    wasAlive = ref Unsafe.Add(ref wasAlive, 1);
                }
            }
        }

        private static void UpdateCache()
        {
            int c = 0;

            var e = ActiveBosses.GetEnumerator();
            while (e.MoveNext())
                ++c;

            _bossCountCache = c;
            _bossActiveCache = c > 0;
        }

        private static void UpdateCacheExcept(NPC ignore)
        {
            UpdateCache();

            if (ignore.active)
            {
                _bossActiveCache = --_bossCountCache > 0;
            }
        }

        private static bool[] _npcWasAlive;
        private static bool _bossActiveCache;
        private static int _bossCountCache;

        internal static void Load()
        {
            _npcWasAlive = new bool[Main.maxNPCs];
        }

        internal static void Unload()
        {
            _npcWasAlive = null;
            _onBossSpawn = null;
        }

        private static NpcSpawnHandler _onBossSpawn;
        private static NpcKillHandler _onBossDie;
        private static NpcKillHandler _onBossDespawn;

        /// <summary>
        /// Invoked after a boss spawns.
        /// </summary>
        public static event NpcSpawnHandler OnBossSpawn
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            add => _onBossSpawn += value;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            remove => _onBossSpawn -= value;
        }

        /// <summary>
        /// Invoked after a boss gets killed.
        /// </summary>
        public static event NpcKillHandler OnBossKill
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            add => _onBossDie += value;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            remove => _onBossDie -= value;
        }

        /// <summary>
        /// Invoked after a boss despawns.
        /// </summary>
        public static event NpcKillHandler OnBossDespawn
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            add => _onBossDespawn += value;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            remove => _onBossDespawn -= value;
        }

        /// <summary>
        /// A collection of every active boss.
        /// </summary>
        public static BossIterator ActiveBosses => new(Main.npc);

        /// <summary>
        /// Any active NPC is a boss.
        /// </summary>
        public static bool IsBossActive => _bossActiveCache;

        /// <summary>
        /// The total amount of active NPCs which count as bosses.
        /// </summary>
        /// <remarks>
        /// Multi-segment bosses like the Eater of Worlds may count as multiple bosses.
        /// </remarks>
        public static int BossCount => _bossCountCache;

        /// <summary>
        /// Decides if this NPC should be treated like a boss NPC.
        /// </summary>
        /// <param name="npc">The NPC to check.</param>
        /// <returns><see langword="true"/> if the NPC should be treated like a boss; otherwise, <see langword="false"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ShouldCountAsBoss(NPC npc)
        {
            return npc.boss || NPCID.Sets.ShouldBeCountedAsBoss[npc.type];
        }

        /// <summary>
        /// Checks to see if any active NPC is a boss and updates <see cref="IsBossActive"/>.
        /// </summary>
        /// <remarks>Typically this doesn't need to be called unless event hooks modify vanilla code.</remarks>
        /// <returns><see langword="true"/> if a boss was found; otherwise, <see langword="false"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool UpdateIsBossActive()
        {
            return _bossActiveCache = ActiveBosses.GetEnumerator().MoveNext();
        }

        /// <summary>
        /// Gets the total amount of active NPCs which are bosses and updates <see cref="BossCount"/>.
        /// </summary>
        /// <remarks>Typically this doesn't need to be called unless event hooks modify vanilla code.</remarks>
        /// <returns>The total number of bosses.</returns>
        public static int UpdateBossCount()
        {
            int c = 0;

            var e = ActiveBosses.GetEnumerator();
            while (e.MoveNext())
                ++c;

            return _bossCountCache = c;
        }

        /// <summary>
        /// Gets the number of active NPCs for which the predicate returns <see langword="true"/>.
        /// </summary>
        /// <remarks>
        /// This function loops through every NPC, so it's recommended to
        /// cache the result somewhere instead of calling the function repeatedly.
        /// </remarks>
        /// <returns>The total number of NPCs for which the predicate returns <see langword="true"/>.</returns>
        public static int CountOf(Predicate<NPC> predicate)
        {
            int c = 0;

            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (predicate(npc))
                    ++c;
            }

            return c;
        }

        /// <summary>
        /// Gets the number of active bosses for which the predicate returns <see langword="true"/>.
        /// </summary>
        /// <remarks>
        /// This function loops through every NPC, so it's recommended to
        /// cache the result somewhere instead of calling the function repeatedly.
        /// </remarks>
        /// <returns>The total number of bosses for which the predicate returns <see langword="true"/>.</returns>
        public static int CountOfBosses(Predicate<NPC> predicate)
        {
            int c = 0;

            foreach (NPC npc in ActiveBosses)
            {
                if (predicate(npc))
                    ++c;
            }

            return c;
        }

        /// <summary>
        /// Gets the total amount of active NPCs which are a segment of the Eater of Worlds.
        /// </summary>
        /// <remarks>
        /// This function loops through every NPC, so it's recommended to
        /// cache the result somewhere instead of calling the function repeatedly.
        /// </remarks>
        /// <returns>The total number of Eater of Worlds segments.</returns>
        public static int GetEaterOfWorldsSegmentCount()
        {
            int c = 0;

            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (npc.type >= NPCID.EaterofWorldsHead && npc.type <= NPCID.EaterofWorldsTail)
                    ++c;
            }

            return c;
        }

        /// <summary>
        /// Gets the total amount of active NPCs which are a creeper.
        /// </summary>
        /// <remarks>
        /// This function loops through every NPC, so it's recommended to
        /// cache the result somewhere instead of calling the function repeatedly.
        /// </remarks>
        /// <returns>The total number of creepers.</returns>
        public static int GetCreeperCount()
        {
            int c = 0;

            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (npc.type == NPCID.Creeper)
                    ++c;
            }

            return c;
        }

        /// <summary>
        /// Gets if the provided NPC type is active right now.
        /// </summary>
        /// <param name="type">The NPC type to check for.</param>
        /// <returns><see langword="true"/> if the type is that of an active NPC; otherwise, <see langword="false"/>.</returns>
        public static bool Exists(int type)
        {
            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (npc.type == type)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Gets if the provided NPC type is active right now.
        /// </summary>
        /// <typeparam name="T">The NPC type to check for.</typeparam>
        /// <returns><see langword="true"/> if the type is that of an active NPC; otherwise, <see langword="false"/>.</returns>
        public static bool Exists<T>() where T : ModNPC
        {
            return Exists(ModContent.NPCType<T>());
        }

        /// <summary>
        /// Gets the number of active NPCs which are of provided type.
        /// </summary>
        /// <param name="type">The NPC type to check for.</param>
        /// <returns>The total number of active NPCs which are of the provided type.</returns>
        public static int CountOf(int type)
        {
            int c = 0;

            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (npc.type == type)
                    ++c;
            }

            return c;
        }

        /// <summary>
        /// Gets the number of active NPCs which are of provided type.
        /// </summary>
        /// <typeparam name="T">The NPC type to check for.</typeparam>
        /// <returns>The total number of active NPCs which are of the provided type.</returns>
        public static int CountOf<T>() where T : ModNPC
        {
            return CountOf(ModContent.NPCType<T>());
        }
    }
}
