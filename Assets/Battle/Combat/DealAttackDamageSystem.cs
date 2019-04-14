﻿using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

namespace Battle.Combat
{
    /// <summary>
    /// Deals damage for all Attack entities with a Damage component.
    /// 
    /// Note that multiple attacks may refer to a single target, so some care is required with concurrently writing to Health.
    /// </summary>
    [   
        UpdateAfter(typeof(FireDirectWeaponsSystem)),
        UpdateBefore(typeof(CleanUpAttacksSystem))
        ]
    public class DealAttackDamageSystem : JobComponentSystem
    {
        private NativeMultiHashMap<Entity, float> m_damageTable;

        /// <summary>
        /// Sorts attack damage into a hash map by target ID.
        /// </summary>
        [BurstCompile]
        struct SortAttackDamageJob : IJobForEachWithEntity<Attack, Target, Damage>
        {
            public NativeMultiHashMap<Entity, float>.Concurrent damageTable;

            public void Execute(
                Entity attack,
                int index,
                [ReadOnly] ref Attack attackFlag,
                [ReadOnly] ref Target target,
                [ReadOnly] ref Damage damage
                )
            {
                damageTable.Add(target.Value, damage.Value);
            }
        }

        /// <summary>
        /// Deals attack damage to all entities with health
        /// </summary>
        [BurstCompile]
        struct DealAttackDamageJob : IJobForEachWithEntity<Health>
        {
            [ReadOnly] public NativeMultiHashMap<Entity, float> damageTable;

            public void Execute(
                Entity target,
                int index,
                ref Health health
                )
            {
                if (!damageTable.TryGetFirstValue(target, out float amount, out var it))
                    return;

                health.Value -= amount;
                while (damageTable.TryGetNextValue(out amount, ref it))
                    health.Value -= amount;
            }
        }

        private bool hasRunAtLeastOnce = false;

        public void TryDisposeNatives()
        {
            if (hasRunAtLeastOnce)
            {
                m_damageTable.Dispose();
            }
        }

        protected override void OnCreateManager()
        {
            m_attackQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new[] {
                    ComponentType.ReadOnly<Attack>(),
                    ComponentType.ReadOnly<Target>(),
                    ComponentType.ReadOnly<Damage>()
                }
            });
        }

        protected EntityQuery m_attackQuery;

        protected override JobHandle OnUpdate(JobHandle inputDependencies)
        {
            TryDisposeNatives();
            m_damageTable = new NativeMultiHashMap<Entity, float>(m_attackQuery.CalculateLength(), Allocator.TempJob);

            var sortJob = new SortAttackDamageJob() { damageTable = m_damageTable.ToConcurrent() };
            var sortJobH = sortJob.Schedule(m_attackQuery, inputDependencies);

            var dealJob = new DealAttackDamageJob() { damageTable = m_damageTable };
            var dealJobH = dealJob.Schedule(this, sortJobH);

            hasRunAtLeastOnce = true;

            return dealJobH;
        }

        protected override void OnStopRunning()
        {
            TryDisposeNatives();
            base.OnStopRunning();
        }
    }
}