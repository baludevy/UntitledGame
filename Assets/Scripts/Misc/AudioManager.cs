using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public static void Play(AudioClip clip, Vector3 position, float minPitch, float maxPitch, float volume = 1, bool d = true)
    {
        if (Instance == null) return;
        Instance.PlayClip(clip, position, minPitch, maxPitch, volume, d);
    }

    private void PlayClip(AudioClip clip, Vector3 position, float minPitch, float maxPitch, float volume, bool d)
    {
        GameObject obj = Instantiate(PrefabManager.Instance.audioPrefab, position, Quaternion.identity);
        AudioSource source = obj.GetComponent<AudioSource>();
        source.clip = clip;
        source.spatialBlend = d ? 1f : 0f;
        source.volume = volume;
        source.pitch = Random.Range(minPitch, maxPitch);
        source.Play();
        Destroy(obj, clip.length / source.pitch);
    }
}