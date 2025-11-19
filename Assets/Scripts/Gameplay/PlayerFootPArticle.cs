using UnityEngine;

public class PlayerFootParticle : MonoBehaviour
{
    [SerializeField] private ParticleSystem footParticle;
    [SerializeField] private float moveThreshold = 0.1f;
    private Vector3 lastPosition;

    private void Start()
    {
        lastPosition = transform.position;
    }

    private void Update()
    {
        float speed = (transform.position - lastPosition).magnitude / Time.deltaTime;

        if (speed > moveThreshold && !footParticle.isPlaying)
        {
            footParticle.Play();
        }
        else if (speed <= moveThreshold && footParticle.isPlaying)
        {
            footParticle.Stop();
        }

        lastPosition = transform.position;
    }
}
