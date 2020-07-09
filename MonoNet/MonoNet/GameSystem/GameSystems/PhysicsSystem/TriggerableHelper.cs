using System.Collections.Generic;
using System.Linq;

namespace MonoNet.GameSystems.PhysicsSystem
{
    /// <summary>
    /// Class for handling all triggerable collisions.
    /// </summary>
    public class TriggerableHelper
    {
        /// <summary>
        /// Helper struct to get a MultiKey for a Dictionary.
        /// </summary>
        public struct MultiKey
        {
            public Rigidbody body1;
            public Rigidbody body2;

            public MultiKey(Rigidbody body1, Rigidbody body2)
            {
                // To have a proper ordering, the first object is the one with a higher hashValue.
                if (body1.GetHashCode() > body2.GetHashCode())
                {
                    this.body1 = body1;
                    this.body2 = body2;
                }
                else
                {
                    this.body1 = body2;
                    this.body2 = body1;
                }
            }
        }

        // All current collisions
        private Dictionary<MultiKey, bool> triggers;

        public TriggerableHelper()
        {
            triggers = new Dictionary<MultiKey, bool>();
        }

        /// <summary>
        /// Adds a new collision to the dictionary.
        /// </summary>
        /// <returns>True if no key was found previously, false if an entry was already found.</returns>
        public bool Add(Rigidbody po1, Rigidbody po2)
        {
            MultiKey key = new MultiKey(po1, po2);
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
            List<MultiKey> keys = triggers.Keys.ToList();
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
            List<MultiKey> keys = triggers.Keys.ToList();
            for (int i = 0; i < keys.Count; i++)
            {
                if (triggers[keys[i]] == false)
                {
                    keys[i].body1.FireEventExit(keys[i].body2);
                    keys[i].body2.FireEventExit(keys[i].body1);
                    triggers.Remove(keys[i]);
                }
            }
        }
    }
}
