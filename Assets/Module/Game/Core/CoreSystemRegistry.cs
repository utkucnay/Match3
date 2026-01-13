public class CoreSystemRegistry : SystemRegistry
{
    LevelData _level;

    private void Awake()
    {
        _level = new LevelData();

        RegisterSystem(_level);
    }
}
