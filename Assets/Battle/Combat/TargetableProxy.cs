﻿using Unity.Entities;
using UnityEngine;

namespace Battle.Combat
{
    [RequiresEntityConversion]
    public class TargetableProxy : MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new Targetable());
        }
    }
}