using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Linq.Expressions;

namespace MonoNet.Network.Commands
{
    public class EventHandlerDictionary : Dictionary<string, EventHandlerEntry>
    {
        /// <summary>
        /// Allows direct access to the EventHandlerDictionary.
        /// </summary>
        public static EventHandlerDictionary Instance { get; private set; }

        public EventHandlerDictionary() {
            Instance = this;

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies) {
                foreach (Type type in assembly.GetTypes()) {
                    try {
                        var allMethods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);

                        IEnumerable<MethodInfo> GetMethods(Type t) {
                            return allMethods.Where(m => m.GetCustomAttributes(t, false).Length > 0);
                        }

                        foreach (var method in GetMethods(typeof(EventHandlerAttribute))) {
                            var parameters = method.GetParameters().Select(p => p.ParameterType).ToArray();
                            var actionType = Expression.GetDelegateType(parameters.Concat(new[] { typeof(void) }).ToArray());
                            var attribute = method.GetCustomAttribute<EventHandlerAttribute>();

                            if (method.IsStatic)
                                EventHandlerDictionary.Instance[attribute.Name] += Delegate.CreateDelegate(actionType, method);
                            else
                                EventHandlerDictionary.Instance[attribute.Name] += Delegate.CreateDelegate(actionType, this, method);
                        }
                    } catch (ReflectionTypeLoadException) { }
                }
            }
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

        internal void Invoke(string eventName, object[] args) {
            var lookupKey = eventName.ToLower();

            if (TryGetValue(lookupKey, out EventHandlerEntry entry))
                entry.Invoke(args);
        }
    }

    public class EventHandlerEntry
    {
        private string name;
        public List<Delegate> callbacks = new List<Delegate>();

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

        /// <summary>
        /// Executes the delegate behind the EventHandlerEntry
        /// </summary>
        /// <param name="args">Arguments to pass into the method.</param>
        /// <returns></returns>
        internal void Invoke(params object[] args) {
            Delegate[] callbacks = this.callbacks.ToArray();

            foreach (Delegate dele in callbacks) {
                try {
                    object[] arguments = CallUtilities.GetPassArguments(dele.Method, args);
                    object retval = dele.DynamicInvoke(arguments);
                } catch (Exception ex) {
                    Console.WriteLine("Error invoking callback for event {0}: {1}", name, ex.ToString());

                    this.callbacks.Remove(dele);
                }
            }
        }
    }

    static class CallUtilities
    {
        public static object[] GetPassArguments(MethodInfo method, object[] args) {
            List<object> passArgs = new List<object>();

            int argIdx = 0;

            object ChangeType(object value, Type type) {
                if (value is null)
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
