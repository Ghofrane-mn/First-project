using UnityEngine;

public class DetectionEchec : MonoBehaviour
{
    [SerializeField] private AudioClip shootSound;

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
        if (collision.gameObject.CompareTag("echec"))
        {

            
            AudioSource audioSource = gameObject.GetComponent<AudioSource>();
            if (audioSource != null)
                audioSource.PlayOneShot(shootSound);

            gameObject.GetComponent<Renderer>().material.color = Color.red;


        }


    }
}
