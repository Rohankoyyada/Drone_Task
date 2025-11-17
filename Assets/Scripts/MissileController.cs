using UnityEngine;

public class MissileController : MonoBehaviour
{
    public ParticleSystem explosionParticle;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Target"))
        {
            Destroy(collision.gameObject);

            ParticleSystem exp = Instantiate(explosionParticle, transform.position, Quaternion.identity);

            Destroy(exp.gameObject, exp.main.duration);  // <- IMPORTANT

            Destroy(gameObject);
        }
    }


}
