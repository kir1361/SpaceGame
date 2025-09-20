using System.Collections;
using UnityEngine;

public class Sounds : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource audioSource;
    public AudioSource musicSource;
    public AudioSource bossSource;
    public AudioSource enemyFightSource;

    private AudioSource loopEffectSource;
    public AudioSource shopMusicSource;
    public static Sounds Instance;

    [Header("Audio Clips")]
    public AudioClip BlackHoleSound;
    public AudioClip PitchSoundEffectClip;
    public AudioClip laserSound, heartSound, deathSound;
    public AudioClip BossExplosionPhase;
    public AudioClip BossDeathExplosion;
    public AudioClip PlayerEnter_ExitBlackHole;
    public AudioClip BackgroundMusic;
    public AudioClip GameOverMusic;
    public AudioClip buttonHitSoundEffectClip;
    public AudioClip laserSoundEffectClip;
    public AudioClip pauseSoundEffectClip;
    public AudioClip noSaveSoundEffectClip;
    public AudioClip enemyHitSoundEffectClip;
    public AudioClip bossMusicSound;
    public AudioClip coinPickupSoundEffectClip;
    public AudioClip coinBuffSoundEffectClip;
    public AudioClip shopMusic;
    public AudioClip buffAddHp;
    public AudioClip buffAddShield;
    public AudioClip buffAddLaser;
    public AudioClip shieldSoundEffectClip;
    public AudioClip enemyFightMusic;
    public AudioClip bossEnterSoundEffectClip;
    public AudioClip enemyEnterSoundEffectClip;
    public AudioClip positiveAnswerSound;
    public AudioClip negativeAnswerSound;
    public AudioClip powerUpSoundEffectClip;
    public AudioClip tradeManagerComingSoundEffectClip;
    public AudioClip parcelPickUpEffectClip;
    public AudioClip countdownSoundEffectClip;





    [Header("Others Settings")]
    public float fadeDuration = 2f;

    public float lastPlaybackTime = 0f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        if (audioSource == null && musicSource == null && loopEffectSource == null && bossSource == null && shopMusicSource == null && enemyFightSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            musicSource = gameObject.AddComponent<AudioSource>();
            loopEffectSource = gameObject.AddComponent<AudioSource>();
            bossSource = gameObject.AddComponent<AudioSource>();
            shopMusicSource = gameObject.AddComponent<AudioSource>();
            enemyFightSource = gameObject.AddComponent<AudioSource>();
        }
    }
    public void PlayMusic(AudioClip clip, float volume = 1f, float pitch = 1f, bool loop = true)
    {
        musicSource.clip = clip;
        musicSource.volume = volume;
        musicSource.pitch = pitch;
        musicSource.loop = loop;
        musicSource.Play();
    }
    public void PlayShopMusic(AudioClip clip, float volume = 1f, float pitch = 1f, bool loop = true)
    {
        shopMusicSource.clip = clip;
        shopMusicSource.volume = volume;
        shopMusicSource.pitch = pitch;
        shopMusicSource.loop = loop;
        shopMusicSource.Play();
        StartCoroutine(Crossfade(musicSource, shopMusicSource, fadeDuration));
    }
    public void PlayBossMusic(AudioClip bossClip)
    {
        bossSource.clip = bossClip;
        bossSource.loop = true;
        bossSource.Play();
        StartCoroutine(Crossfade(musicSource, bossSource, fadeDuration));
    }
    public void PlayEnemyMusic(AudioClip enemyClip)
    {
        enemyFightSource.clip = enemyClip;
        enemyFightSource.loop = true;
        enemyFightSource.Play();
        StartCoroutine(Crossfade(musicSource, enemyFightSource, fadeDuration));
    }

    public void PlaySoundEffect(AudioClip clip, float volume = 1f, float p1 = 1.15f, float p2 = 1.15f)
    {
        audioSource.pitch = Random.Range(p1, p2);
        audioSource.PlayOneShot(clip, volume);
    }
    public void PlayLoopEffect(AudioClip clip, float volume = 1f, float pitch = 1f)
    {
        loopEffectSource.clip = clip;
        loopEffectSource.volume = volume;
        loopEffectSource.pitch = pitch;
        loopEffectSource.loop = true;
        loopEffectSource.Play();
    }
    public void StopLoopEffect()
    {
        if (loopEffectSource.isPlaying)
        {
            loopEffectSource.Stop();
        }
    }
    public IEnumerator FadeOut(AudioSource source, float duration = 1f)
    {
        float startVolume = source.volume;

        while (source.volume > 0)
        {
            source.volume -= startVolume * Time.deltaTime / duration;
            yield return null;
        }

        lastPlaybackTime = source.time; // Store the last playback time before stopping
        source.Stop();
        source.volume = startVolume;
    }

    public IEnumerator Crossfade(AudioSource from, AudioSource to, float duration)
    {
        to.Play();
        float time = 0f;
        to.volume = 0f; // Start with the new music at 0 volume

        float fromStartVolume = from.volume;
        float toStartVolume = to.volume;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            from.volume = Mathf.Lerp(fromStartVolume, 0f, t);
            to.volume = Mathf.Lerp(toStartVolume, 0.05f, t);
            yield return null;
        }

        from.Stop();
        if (from.name == "musicSource")
        {
            lastPlaybackTime = from.time; // Store the last playback time before stopping // Не забыть потом запустить с этого времени
        }
        from.volume = fromStartVolume;

    }
    public void StopAllMusic()
    {
        musicSource.Stop();
        bossSource.Stop();
        enemyFightSource.Stop();
        shopMusicSource.Stop();
    }
}
