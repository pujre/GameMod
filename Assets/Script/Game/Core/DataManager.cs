public class DataManager : SingletonMono<DataManager>
{
	public void SetData(OnDataKey key,int x)
	{
		PlayerPrefs.SetInt(key.ToString(),x);
	}

	public int GetData(OnDataKey key) {
		return PlayerPrefs.GetInt(key.ToString(),0);
	}

	
}
