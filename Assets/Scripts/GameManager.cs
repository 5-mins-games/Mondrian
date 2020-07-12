using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager instance;

	[Range(3, 5)]
	public int maxLevel = 5;

	private int level;


	void Awake()
	{
		if (instance != null && this != instance)
		{
			Destroy(this.gameObject);
		}
		instance = this;

		Debug.Log("GameManager initialized. instance: " + instance);
	}



	#region Level Up

	public void ClearLevel()
	{
		if (!Map.instance.ClearLevel()) return;

		ProgressLevel();
	}

	public void ProgressLevel()
	{
		if (level >= maxLevel)
		{
			EndLevel();

			return;
		}
		level++;

        Debug.Log("Generate new level: " + level);

		Map.instance.ProgressLevel(LevelDiff.config[level]);
	}

	public void EndLevel()
	{
		Debug.Log("Show credits and exit.");

		Application.Quit();
	}
	#endregion
}
