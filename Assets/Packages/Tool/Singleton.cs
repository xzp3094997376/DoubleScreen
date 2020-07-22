public class Singleton<T> where T : class, new()
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new T();
            }
            return instance;
        }
    }
    public void Dispose()
    {
        instance = null;
    }
    public virtual void Update(float fTime, float fDTime)
    {

    }
    public virtual void LateUpdate(float fTime, float fDTime)
    {

    }
    public virtual void FixedUpdate(float fTime, float fDTime)
    {

    }
    public virtual void OnDestory()
    {

    }

}
