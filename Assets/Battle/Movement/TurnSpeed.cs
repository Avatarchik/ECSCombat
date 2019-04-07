﻿using System;
using Unity.Entities;
using UnityEngine;

namespace Battle.Movement
{
    /// <summary>
    /// Maximum speed at which an entity can turn.
    /// </summary>
    [Serializable]
    public struct TurnSpeed : IComponentData
    {
        public float RadiansPerSecond;
    }
}