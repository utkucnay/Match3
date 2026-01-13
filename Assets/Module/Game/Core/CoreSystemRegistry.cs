public class CoreSystemRegistry : SystemRegistry
{
    Level _level;

    private void Awake()
    {
        _level = new Level();

        RegisterSystem(_level);
    }
}
