using UnityEngine;

public class Effect : MonoBehaviour
{
    private ParticleSystem[] _effects;

    void Start()
    {
        _effects = GetComponentsInChildren<ParticleSystem>();
    }

    public void Show(bool toShow, bool stopImmediately = false)
    {
        if (toShow)
        {
            Play();
        }
        else
        {
            Stop(stopImmediately);
        }
    }

    public void Play()
    {
        for (int i = 0; i < _effects.Length; i++)
        {
            _effects[i].Play();
        }
    }

    public void Stop(bool stopImmediately)
    {
        for (int i = 0; i < _effects.Length; i++)
        {
            if (stopImmediately)
            {
                _effects[i].Clear();
            }
            _effects[i].Stop();
        }
    }
}
