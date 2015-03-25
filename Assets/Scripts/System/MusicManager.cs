using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour {

    public AudioClip gameOverMusic;
	public AudioClip levelEndMusic;
	public AudioClip rankingScreenMusic;
    public AudioClip[] musicList;
    private AudioSource musicSource;
    private int musicIndex = 0;
    private bool looping = false;
    private bool gameOver = false;
	private bool levelEnd = false;

	// Use this for initialization
	void Start () 
    {
        musicSource = GetComponent<AudioSource>();

        musicSource.loop = false;
        CycleTrack();
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (!musicSource.isPlaying && !looping && !gameOver && !levelEnd)
        {
            if (musicIndex >= musicList.Length - 1)
            {
                LoopTrack();
            }
            else
            {
                musicIndex++;
                CycleTrack();
            }
        }
	}

    public void CycleTrack()
    {
        musicSource.clip = musicList[musicIndex];
        musicSource.Play();
    }

    public void LoopTrack()
    {
        musicSource.clip = musicList[musicList.Length - 1];
        musicSource.loop = true;
        looping = true;
        musicSource.Play();
    }

    public void PlayGameOver()
    {
        //iTween.AudioTo(gameObject,1,0,7);
        gameOver = true;
        musicSource.Stop();
        musicSource.PlayOneShot(gameOverMusic);
    }

	public void PlayLevelEnd()
	{
		//iTween.AudioTo(gameObject,1,0,7);
		levelEnd = true;
		musicSource.Stop();
		musicSource.PlayOneShot(levelEndMusic);
	}

	public void PlayRankingScreen()
	{
		//iTween.AudioTo(gameObject,1,0,7);
		musicSource.loop = true;
		musicSource.Stop();
		musicSource.clip = rankingScreenMusic;
		musicSource.Play();
	}
}
