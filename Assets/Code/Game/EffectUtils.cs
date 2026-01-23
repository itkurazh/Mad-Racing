using UnityEngine;

public static class EffectUtils
{
    public static void Play(this ParticleSystem[] particles)
    {
        foreach (var particle in particles)
            particle.Play();
    }
    
    public static void Stop(this ParticleSystem[] particles)
    {
        foreach (var particle in particles)
            particle.Stop();
    }
}