using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Music")]
    [SerializeField] AudioSource musicSource;

    [Header("SFX")]
    [SerializeField] AudioSource sfxSource;

    [Header("Clips")]
    [SerializeField] AudioClip jumpSFX;
    [SerializeField] AudioClip shootSFX;
    [SerializeField] AudioClip hitSFX;
    [SerializeField] AudioClip deathSFX;
    [SerializeField] AudioClip collectSFX;

    [SerializeField] AudioClip enemyfinishSFX;

    [SerializeField] AudioClip winSFX;

    [SerializeField] AudioClip playerDamageSFX;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void PlayJump()  => sfxSource.PlayOneShot(jumpSFX);
    public void PlayShoot() => sfxSource.PlayOneShot(shootSFX);
    public void PlayHit()   => sfxSource.PlayOneShot(hitSFX);
    public void PlayDeath() => sfxSource.PlayOneShot(deathSFX);
    public void PlayCollect() => sfxSource.PlayOneShot(collectSFX);

    public void PlayFinish() => sfxSource.PlayOneShot(enemyfinishSFX);

    public void PlayWin() => sfxSource.PlayOneShot(winSFX);

     public void PlayDamage() => sfxSource.PlayOneShot(playerDamageSFX);

    public void SetMusicVolume(float volume) => musicSource.volume = volume;

    public void StopMusic()
{
    if (musicSource.isPlaying)
        musicSource.Stop();
}

public void PlayMusic()
{
    if (!musicSource.isPlaying)
        musicSource.Play();
}

}