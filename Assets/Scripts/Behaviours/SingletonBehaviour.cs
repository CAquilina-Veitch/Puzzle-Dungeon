using UnityEngine;

namespace Scripts.Behaviours
{
    /// <summary>
    /// Generic singleton base class for MonoBehaviours.
    /// Inherit from this to make any MonoBehaviour a singleton.
    /// </summary>
    /// <typeparam name="T">The type of the singleton class</typeparam>
    public abstract class SingletonBehaviour<T> : MonoBehaviour where T : SingletonBehaviour<T>
    {
        private static T _instance;

        public static T Instance => _instance;
        
        /// <summary>
        /// Checks if an instance of this singleton exists.
        /// </summary>
        public static bool HasInstance => _instance != null;
        
        /// <summary>
        /// Controls whether this singleton persists between scenes.
        /// Override this in derived classes to change behavior.
        /// </summary>
        protected virtual bool DontDestroyOnLoad => true;
        
        /// <summary>
        /// Controls whether to destroy duplicate instances or the original.
        /// If true, new instances are destroyed. If false, the original is destroyed.
        /// </summary>
        protected virtual bool DestroyDuplicates => true;

        protected virtual void Awake()
        {
            if (_instance != null && _instance != this)
            {
                if (DestroyDuplicates)
                {
                    Destroy(gameObject);
                    return;
                }
                else
                    Destroy(_instance.gameObject);
            }
            
            _instance = this as T;
            
            if (DontDestroyOnLoad && transform.parent == null) DontDestroyOnLoad(gameObject);
            
            OnAwake();
        }
        
        /// <summary>
        /// Override this instead of Awake() in derived classes.
        /// This is called after singleton initialization.
        /// </summary>
        protected virtual void OnAwake() { }

        protected virtual void OnDestroy()
        {
            if (_instance == this) _instance = null;
            
            OnBeforeDestroy();
        }
        
        /// <summary>
        /// Override this for cleanup logic in derived classes.
        /// </summary>
        protected virtual void OnBeforeDestroy() { }
        
        /// <summary>
        /// Ensures an instance exists. Creates one if it doesn't.
        /// Useful for singletons that should always exist.
        /// </summary>
        public static T EnsureInstance()
        {
            if (_instance == null)
            {
                var go = new GameObject($"{typeof(T).Name} (Singleton)");
                _instance = go.AddComponent<T>();
            }
            
            return _instance;
        }
    }
}