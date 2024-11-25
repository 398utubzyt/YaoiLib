using Mono.Cecil;

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

        public ref struct Enumerator(ReadOnlySpan<NPC>.Enumerator enumerator)
        {
            private ReadOnlySpan<NPC>.Enumerator _enumerator = enumerator;

            public readonly NPC Current => _enumerator.Current;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool MoveNext()
            {
                while (_enumerator.MoveNext())
                {
                    if (_enumerator.Current.active && _enumerator.Current.boss)
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
        private class NpcHelperGlobalNPC : GlobalNPC
        {
            public override void OnSpawn(NPC npc, IEntitySource source)
            {
                if (npc.boss)
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
                    if (npc.boss)
                    {
                        _onBossDie?.Invoke(npc);
                        _npcWasAlive[npc.whoAmI] = false;
                        UpdateCache();
                    }
                }
            }

            public override void HitEffect(NPC npc, NPC.HitInfo hit)
            {
                if (NetHelper.IsClient)
                {
                    if (npc.boss && npc.life <= 0)
                    {
                        _onBossDie?.Invoke(npc);
                        _npcWasAlive[npc.whoAmI] = false;
                        UpdateCache();
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
            _bossActiveCache = UpdateBossCount() > 0;
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
        /// The total amount of active NPCs which are bosses.
        /// </summary>
        public static int BossCount => _bossCountCache;

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
    }
}
