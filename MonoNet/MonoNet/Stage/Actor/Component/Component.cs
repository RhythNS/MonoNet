using System.Collections;

namespace MonoNet.Stage.Actor
{
    public abstract class Component
    {
        public Actor Actor { get; private set; }

        public void Initialize(Actor actor)
        {
            Actor = actor;
        }

        public Coroutine StartCoroutine(IEnumerator enumerator)
        {
            return null;
        }
    }
}
