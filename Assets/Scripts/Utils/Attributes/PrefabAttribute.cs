using UnityEngine;
using System;

namespace Mio.Utils {
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class PrefabAttribute : Attribute {
        public readonly string Name;
        public readonly bool Persistent;

        public PrefabAttribute(string name, bool persistent) {
            Name = name;
            Persistent = persistent;
        }

        public PrefabAttribute(string name) {
            Name = name;
            Persistent = false;
        }
    }
}