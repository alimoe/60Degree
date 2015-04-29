namespace Core
{
    public abstract class Singleton<T>
        where T: class,  new()
    {
        public static T Instance
        {
            get
            {
                if (s_instance == null)
                {
					//DebugUtils.Assert(LogCategory.System, !s_reentryGuard, "Reentrant singleton instance initialization");
					s_reentryGuard = true;
                    s_instance = new T();
					s_reentryGuard = false;
                }

                return s_instance;
            }
        }
		
		public static bool Exists
		{
			get { return null != s_instance; }
		}

        protected static T s_instance = null;
		protected static bool s_reentryGuard = false;
		
    }


    public abstract class ControlledSingleton<T>
        where T: class,  new()
    {
        public static T Instance
        {
            get
            {
                return s_instance;
            }
        }
		
		public static bool Exists
		{
			get { return null != s_instance; }
		}
		
		public static T Instantiate() 
		{
			//DebugUtils.Assert(LogCategory.Framework, null == s_instance);
			{
				//DebugUtils.Assert(LogCategory.System, !s_reentryGuard, "Reentrant singleton instance initialization");
				s_reentryGuard = true;
	            s_instance = new T();
				s_reentryGuard = false;
			}
			
			return s_instance;
		}

        protected static T s_instance = null;
		protected static bool s_reentryGuard = false;
		
    }

}