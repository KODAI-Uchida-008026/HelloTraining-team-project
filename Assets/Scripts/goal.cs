using UnityEngine;

public class Goal : MonoBehaviour
{
    public ParticleSystem goalParticle;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            goalParticle.Play();
        }
    }
}