using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace MonoNet.Network.Commands
{
    public class EventHandlerDictionary : Dictionary<string, EventHandlerEntry>
    {
        public static EventHandlerDictionary Instance { get; private set; }
        public EventHandlerDictionary() {
            Instance = this;
        }

        public new EventHandlerEntry this[string key] {
            get {
                var lookupKey = key.ToLower();

                if (this.ContainsKey(lookupKey))
                    return base[lookupKey];

                EventHandlerEntry entry = new EventHandlerEntry(key);
                base.Add(lookupKey, entry);

                return entry;
            }
            set { }
        }

        public void Add(string key, Delegate value) {
            this[key] += value;
        }

        internal async Task Invoke(string eventName, string sourceString, object[] args) {
            var lookupKey = eventName.ToLower();
            EventHandlerEntry entry;

            if (TryGetValue(lookupKey, out entry))
                await entry.Invoke(sourceString, args);
        }
    }

    public class EventHandlerEntry
    {
        private string name;
        private List<Delegate> callbacks = new List<Delegate>();

        public EventHandlerEntry(string name) {
            this.name = name;
        }

        public static EventHandlerEntry operator +(EventHandlerEntry entry, Delegate dele) {
            entry.callbacks.Add(dele);

            return entry;
        }
        public static EventHandlerEntry operator -(EventHandlerEntry entry, Delegate dele) {
            entry.callbacks.Remove(dele);

            return entry;
        }

        internal async Task Invoke(string sourceString, params object[] args) {
            Delegate[] callbacks = this.callbacks.ToArray();

            foreach (Delegate dele in callbacks) {
                try {
                    object[] arguments = CallUtilities.GetPassArguments(dele.Method, args, sourceString);
                    object retval = dele.DynamicInvoke(arguments);

                    if (retval != null && retval is Task task) {
                        await task;
                    }
                } catch (Exception ex) {
                    Console.WriteLine("Error invoking callback for event {0}: {1}", name, ex.ToString());

                    this.callbacks.Remove(dele);
                }
            }
        }
    }

    static class CallUtilities
    {
        public static object[] GetPassArguments(MethodInfo method, object[] args, string sourceString) {
            List<object> passArgs = new List<object>();

            int argIdx = 0;

            object ChangeType(object value, Type type) {
                if (ReferenceEquals(value, null))
                    return null;

                if (type.IsAssignableFrom(value.GetType()))
                    return value;
                else if (value is IConvertible)
                    return Convert.ChangeType(value, type);

                throw new InvalidCastException($"Could not cast event argument from {value.GetType().Name} to {type.Name}");
            }

            object Default(Type type) => type.IsValueType ? Activator.CreateInstance(type) : null;

            foreach (var info in method.GetParameters()) {
                var type = info.ParameterType;

                if (argIdx >= args.Length)
                    passArgs.Add(Default(type));
                else
                    passArgs.Add(ChangeType(args[argIdx], type));

                argIdx++;
            }

            return passArgs.ToArray();
        }
    }
}
