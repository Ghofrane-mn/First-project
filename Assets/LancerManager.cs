using TMPro;
using UnityEngine;

public class LancerManager : MonoBehaviour
{

    private GameObject[] Objects;
    private GameObject[] targets;

    private bool reussi;

    [SerializeField] private TextMeshPro myText;
    [SerializeField] private AudioClip shootSound;

    private Color colorbase;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        Objects = GameObject.FindGameObjectsWithTag("Objects");
        Debug.Log("Objects trouvées : " + Objects.Length);

        targets = GameObject.FindGameObjectsWithTag("Target");
        Debug.Log("Cibles trouvées : " + targets.Length);
        colorbase = gameObject.GetComponent<Renderer>().material.color;




    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Objects"))
        {

            reussi = true;
            Debug.Log("Cible touchée !");
            float number = float.Parse(myText.text) + 1;
            myText.text = number.ToString();
            AudioSource audioSource = gameObject.GetComponent<AudioSource>();
            if (audioSource != null)
                audioSource.PlayOneShot(shootSound);

            gameObject.GetComponent<Renderer>().material.color = Color.green;


        }


    }


    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.CompareTag("Objects"))
        {
            

            gameObject.GetComponent<Renderer>().material.color = colorbase;


        }
    }

}
