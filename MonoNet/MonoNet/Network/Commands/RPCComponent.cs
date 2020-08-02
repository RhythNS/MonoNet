using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MonoNet.Network;
using MonoNet.Network.Commands;

namespace MonoNet.Testing.NetTest
{
    class RPCComponent : NetSyncComponent
    {
        protected override void OnInitialize() {
            base.OnInitialize();

            var allMethods = this.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);

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
        }

        public static void TriggerServerEvent(string eventName, params object[] args) {
            // only execute this on client

            // TODO: communication to server
        }

        public static void TriggerClientEvent(/*player to trigger this on*/int playerId, string eventName, params object[] args) {
            // only execute this on server

            // TODO: communication to client
        }

        public static void TriggerClientEvent(string eventName, params object[] args) {
            // only execute this on server

            // TODO: trigger on all connected clients
        }

        // How an event should look
        //[EventHandler("TEST")]
        //private void Test(int test) {
        //    // Do something...
        //}
    }
}
