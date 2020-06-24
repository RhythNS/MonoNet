using MonoNet.ECS;
using MonoNet.Util;
using System.Collections;

namespace MonoNet.Testing.Infrastructure
{
    public class CoroutineTest : Component
    {
        protected override void OnInitialize()
        {
            StartCoroutine(PrintCoroutineTestEverSecond());
        }

        private IEnumerator PrintCoroutineTestEverSecond()
        {
            while (true)
            {
                Log.Info("Coroutine yaaaay");
                yield return new WaitForSeconds(1);
            }
        }
        

        private void OnFinsh()
        {
            Log.Info("Finished!");
        }
    }
}
