/// <summary>
/// Generic Save Profile:
/// Decouples SaveController from concrete Save ...Data types
/// </summary>
public interface ISaveProfile<TSave> where TSave : new()
{
    TSave ExtractSaveData();
    void ApplySaveData(TSave data, bool notifyChange);
}
