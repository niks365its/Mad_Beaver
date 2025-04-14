using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    public AudioSource audioSourceMusic;
    public AudioSource audioSourceEffects;
    public AudioSource audioSourceWalk;

    public AudioClip backgroundSound, windSound, walkSound, shotSound, mushroomSound, hitSound, jumpSound, flyStickSound, emptyStickSound, fallSound, woodGetSound, sharpSound, treesTrapSound, fireworkSound, deathSound, gameOverSound, hintSound, airSound;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // DontDestroyOnLoad(gameObject);
    }

    // private void Start()
    // {
    //     SoundManager.Instance.audioSource.volume = 0.5f; // 50% гучності  
    //     PlayBackgroundMusic();
    // }

    public void PlayBackgroundMusic(AudioClip backgroundClip)
    {
        if (backgroundClip != null && audioSourceMusic.clip != backgroundClip)
        {
            audioSourceMusic.clip = backgroundClip;
            audioSourceMusic.loop = true;
            audioSourceMusic.Play();
        }
    }

    public void PlayWalkSound()
    {
        if (!audioSourceWalk.isPlaying)
        {
            audioSourceWalk.clip = walkSound;
            audioSourceWalk.loop = true;
            audioSourceWalk.Play();
        }
    }

    public void StopWalkSound()
    {
        if (audioSourceWalk != null && audioSourceWalk.isPlaying)
        {
            audioSourceWalk.Stop();
        }
    }

    public void PlayOneShot(AudioClip clip)
    {
        if (clip != null)
        {
            audioSourceWalk.Stop(); // Перериваємо звук кроків
            audioSourceEffects.PlayOneShot(clip);
        }
    }

    public void PlaySound(AudioClip clip)
    {
        audioSourceWalk.Stop(); // Перериваємо звук кроків
        audioSourceEffects.clip = clip; // Призначаємо кліп
        audioSourceEffects.loop = true; // Встановлюємо повтор
        audioSourceEffects.Play(); // Програємо звук
    }


    public void StopSound()
    {

        audioSourceEffects.Stop();

    }

    public void StopEffect(AudioClip clip)
    {

        if (audioSourceEffects.isPlaying && audioSourceEffects.clip == clip)
        {
            audioSourceEffects.Stop();
        }
    }


    public void StopEffectsSound()
    {
        if (audioSourceEffects.isPlaying)
        {
            audioSourceEffects.Stop(); // Перериваємо звук кроків
        }
    }

    public void StopBackgroundSound()
    {
        if (audioSourceMusic.isPlaying)
        {
            audioSourceMusic.Stop(); // Перериваємо звук кроків
        }
    }


}
