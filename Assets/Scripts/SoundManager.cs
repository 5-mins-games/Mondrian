using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
	public AudioClip clickSFX;

	private AudioSource audio;

	public static SoundManager instance;

	void Start()
	{
		if (instance != null && instance != this)
		{
			Destroy(this.gameObject);
		}
		instance = this;

		audio = GetComponent<AudioSource>();
	}

	public void PlayClick()
	{
		if (clickSFX == null) return;

		Debug.Log("Play click SFX.");

		audio.PlayOneShot(clickSFX);
	}
}
