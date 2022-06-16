using System;
using UnityEngine;

namespace NiftyFramework.Core.PrefabReferences
{
    public class PrefabReferenceAttribute : PropertyAttribute
    {
        public readonly Type Type;

        public PrefabReferenceAttribute(Type type)
        {
            Type = type;
        }
    }
}