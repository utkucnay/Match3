public class CoreSystemManager : SystemManager
{
    Level level;

    private void Awake()
    {
        level = new Level();

        RegisterSystem(level);
    }
}