public class DataManager : SingletonMono<DataManager>
{
	public void SetData(OnDataKey key, int x)
	{
		PlayerPrefs.SetInt(key.ToString(), x);
	}

	public int GetData(OnDataKey key)
	{
		return PlayerPrefs.GetInt(key.ToString(), 0);
	}

	public int GetHighestLevel()
	{
		return PlayerPrefs.GetInt(OnDataKey.HighestLevel.ToString(), 1);
	}

	public void SetHighestLevel(int x)
	{
		 PlayerPrefs.SetInt(OnDataKey.HighestLevel.ToString(), x);
	}
}