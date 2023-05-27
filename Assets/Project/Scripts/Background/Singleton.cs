using UnityEngine;

namespace Project.Scripts.Background
{
    /// <summary>
    ///   <para>class driving form this class will act as Singletons</para>
    /// </summary>

    public abstract class Singleton<T> : MonoBehaviour where T :MonoBehaviour
    {
        private static T internalInstance;
        public static T Instance
        {
            get => internalInstance;
            private set => internalInstance = value;
        }

        protected virtual void Awake()
        {
            if (!Instance) Instance = gameObject.GetComponent<T>();
            else if(Instance.GetInstanceID() != GetInstanceID())
            {
                Destroy(gameObject.GetComponent<T>());
            }
        }
    }
}