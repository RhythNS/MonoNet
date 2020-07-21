using MonoNet.Util.Datatypes;
using System.Collections.Generic;
using System.Linq;

namespace MonoNet.GameSystems.PhysicsSystem
{
    /// <summary>
    /// Class for handling all triggerable collisions.
    /// </summary>
    public class TriggerableHelper
    {
        // All current collisions
        private Dictionary<MultiKey<Rigidbody>, bool> triggers;

        public TriggerableHelper()
        {
            triggers = new Dictionary<MultiKey<Rigidbody>, bool>();
        }

        /// <summary>
        /// Adds a new collision to the dictionary.
        /// </summary>
        /// <returns>True if no key was found previously, false if an entry was already found.</returns>
        public bool Add(Rigidbody po1, Rigidbody po2)
        {
            MultiKey<Rigidbody> key = new MultiKey<Rigidbody>(po1, po2);
            if (triggers.ContainsKey(key) == false)
            {
                triggers.Add(key, true);
                return true;
            }
            else
            {
                triggers[key] = true;
                return false;
            }
        }

        /// <summary>
        /// Sets all keys to false.
        /// </summary>
        public void SetAllToNonTouching()
        {
            List<MultiKey<Rigidbody>> keys = triggers.Keys.ToList();
            for (int i = 0; i < keys.Count; i++)
            {
                triggers[keys[i]] = false;
            }
        }

        /// <summary>
        /// Removes all keys that have a value of false and sends a OnTriggerExit to the two objects inside.
        /// the key.
        /// </summary>
        public void RemoveAllNonTouching()
        {
            List<MultiKey<Rigidbody>> keys = triggers.Keys.ToList();
            for (int i = 0; i < keys.Count; i++)
            {
                if (triggers[keys[i]] == false)
                {
                    keys[i].object1.FireEventExit(keys[i].object2);
                    keys[i].object2.FireEventExit(keys[i].object1);
                    triggers.Remove(keys[i]);
                }
            }
        }
    }
}
