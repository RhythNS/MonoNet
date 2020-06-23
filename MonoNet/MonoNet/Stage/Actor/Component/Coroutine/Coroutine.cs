using MonoNet.GameSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoNet.Stage.Actor
{
    public class Coroutine
    {
        public static IEnumerator WaitForSeconds(float seconds)
        {
            float timer = 0;
            while (timer < seconds)
            {
                timer += Time.Delta;
                yield return null;
            }
        }
    }
}
