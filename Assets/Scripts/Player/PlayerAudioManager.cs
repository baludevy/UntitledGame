using UnityEngine;

[System.Serializable]
public class Sound
{
    public AudioClip clip;
    public bool randomPitch;
    [Range(0.5f, 2f)] public float minPitch = 0.9f;
    [Range(0.5f, 2f)] public float maxPitch = 1.1f;
}


public class PlayerAudioManager : MonoBehaviour
{
    public static PlayerAudioManager Instance;
    public Sound[] sounds;
    private AudioSource source;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        source = gameObject.AddComponent<AudioSource>();
        source.spatialBlend = 0f;
        source.playOnAwake = false;
    }

    public void PlaySound(int index)
    {
        if (index < 0 || index >= sounds.Length) return;

        Sound s = sounds[index];
        if (s.clip == null) return;

        if (s.randomPitch)
            source.pitch = Random.Range(s.minPitch, s.maxPitch);
        else
            source.pitch = 1f;

        source.PlayOneShot(s.clip);
    }
}