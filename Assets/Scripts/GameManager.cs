using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour
{
	public static GameManager instance;

	[Range(3, 5)]
	public int maxLevel = 5;

	private Text tutorial;
	private int elapse;
	private int maxElapse;
	private readonly Color origin = new Color(0.85f, 0.85f, 0.85f, 1.0f);
	private readonly Color fade   = new Color(0.33f, 0.33f, 0.33f, 0.1f);

	internal bool locked = false;

	private int level;



	#region MonoBehaviour

	private void Awake()
	{
		if (instance != null && this != instance)
		{
			Destroy(this.gameObject);
		}
		instance = this;

		Debug.Log("GameManager initialized. instance: " + instance);
	}

	private void Start()
	{
		tutorial = GameObject.Find("Tutorial").GetComponent<Text>();
	}

	private void Update()
	{
		FadeText();
	}
	#endregion



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

		IEnumerator coro = ProgressLevelDelay(LevelDiff.config[level], 2.0f);

		StartCoroutine(coro);

		ShowText(LevelText.text[level]);
	}

	private IEnumerator ProgressLevelDelay(LevelConf config, float delay)
	{
		locked = true;

		while (true)
        {
            yield return new WaitForSeconds(delay);

	        Map.instance.ProgressLevel(config);
	        break;
        }
        locked = false;

        yield return null;
	}

	public void EndLevel()
	{
		ShowText(LevelText.text[level + 1], 2000);

		Map.instance.Destroy();
	}
	#endregion



	#region Game Management

	public void ExitGame()
	{
		Application.Quit();
	}

	public void RestartGame()
	{
		Map.instance.Destroy();

		level = 0;

		ProgressLevel();
	}
	#endregion



	#region Tutorial Text

	private void ShowText(string text, int elapse=500)
	{
		tutorial.text = text;
		
		this.elapse = elapse;
		this.maxElapse = elapse;
	}

	private void FadeText()
	{
		if (elapse > 0)
		{
			elapse--;
			tutorial.color = Color.Lerp(origin, fade, 1.0f - (float) elapse / (float) maxElapse);
		}
		else
		{
			tutorial.color = origin;
			tutorial.text = "";
		}
	}
	#endregion
}
