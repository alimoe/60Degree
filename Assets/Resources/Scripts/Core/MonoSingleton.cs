using UnityEngine;
using System.Collections;

namespace Core
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        protected static T s_instance = null;

        public static T Instance 
        {
            get
            {
                //DebugUtils.Assert(Core.LogCategory.System, s_instance != null, typeof(T).ToString() + "has not been created. Please add the component first ");
                return s_instance;
            }
        }

        protected virtual void Awake()
        {
            if (s_instance == null)
            {
                s_instance = this as T;
            }
        }

        private void OnApplicationQuit()
        {
            s_instance = null;
        }
    }
	
	
	
    public abstract class MonoStrictSingleton<T> : MonoSingleton<T> where T : MonoSingleton<T>	
	{
	    protected override void Awake()
		{
			//Debug.Log(LogCategory.Audio, null == Instance);
			base.Awake();
			//DebugUtils.Assert(LogCategory.Audio, this == Instance);
		}
		
		public virtual void ReleaseSingleton()
		{
			//DebugUtils.Assert(LogCategory.Audio, this == Instance);
			s_instance = null;
		}
		
		protected virtual void OnDestroy()
		{
			if (this == Instance)
			{
				ReleaseSingleton();
			}
		}

	}
}
