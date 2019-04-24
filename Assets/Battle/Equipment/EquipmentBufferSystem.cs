﻿using Unity.Entities;
using UnityEngine;

namespace Battle.Equipment
{
    /// <summary>
    /// Applies changes to entities due to equipment attachment/removal.
    /// </summary>
    [ExecuteAlways]
    [UpdateAfter(typeof(EquipmentUpdateGroup))]
    public class EquipmentBufferSystem : EntityCommandBufferSystem
    {
    }
}
