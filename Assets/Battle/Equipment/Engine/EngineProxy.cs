﻿using Unity.Entities;
using UnityEngine;

namespace Battle.Equipment
{
    [RequiresEntityConversion]
    [RequireComponent(typeof(EnabledProxy))]
    public class EngineProxy : MonoBehaviour, IConvertGameObjectToEntity
    {
        public float Thrust;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var data = new Engine { Thrust = Thrust };
            dstManager.AddComponentData(entity, data);
        }
    }
}