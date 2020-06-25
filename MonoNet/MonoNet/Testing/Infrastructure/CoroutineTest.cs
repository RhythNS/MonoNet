using MonoNet.ECS;
using MonoNet.Util;
using System.Collections;

namespace MonoNet.Testing.Infrastructure
{
    public class CoroutineTest : Component
    {
        protected override void OnInitialize()
        {
            StartCoroutine(CustomEnum(), OnFinsh);
        }

        private IEnumerator CustomEnum()
        {
            yield return Print();
            yield return Print2();
            yield return Print3();
        }

        private IEnumerator Print()
        {
            Log.Info("Coroutine 1");
            yield return new WaitForSeconds(1);
        }


        private IEnumerator Print2()
        {
            Log.Info("Coroutine 2");
            yield return new WaitForSeconds(1);
        }


        private IEnumerator Print3()
        {
            Log.Info("Coroutine 3");
            yield return new WaitForSeconds(1);
        }

        private void OnFinsh()
        {
            Log.Info("Coroutine finished!");
        }
    }
}
