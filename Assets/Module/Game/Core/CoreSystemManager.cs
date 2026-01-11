public class CoreSystemManager : SystemManager
{
    Level _level;

    private void Awake()
    {
        _level = new Level();

        RegisterSystem(_level);
    }
}