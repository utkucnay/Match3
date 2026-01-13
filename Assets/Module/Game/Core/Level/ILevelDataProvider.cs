public interface ILevelDataProvider
{
    LevelData LoadLevel(int id);
    LevelData LoadLevel(string name);
}
