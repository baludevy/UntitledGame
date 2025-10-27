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

    public static void Play3D(AudioClip clip, Vector3 position, float minPitch, float maxPitch, float volume = 1)
    {
        if (Instance == null) return;
        Instance.PlayClip3D(clip, position, minPitch, maxPitch, volume);
    }

    private void PlayClip3D(AudioClip clip, Vector3 position, float minPitch, float maxPitch, float volume)
    {
        GameObject obj = Instantiate(PrefabManager.Instance.audioPrefab, position, Quaternion.identity);
        AudioSource source = obj.GetComponent<AudioSource>();
        source.clip = clip;
        source.volume = volume;
        source.pitch = Random.Range(minPitch, maxPitch);
        source.Play();
        Destroy(obj, clip.length / source.pitch);
    }

}