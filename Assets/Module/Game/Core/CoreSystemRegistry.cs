public class CoreSystemRegistry : SystemRegistry
{
    Level _level;

    protected override void Awake()
    {
        base.Awake();

        _level = new Level();

        RegisterSystem(_level);
    }
}
