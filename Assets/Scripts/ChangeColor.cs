using UnityEngine;

public class ChangeColor : MonoBehaviour
{

    Color colorbase;
    public AudioSource source;
    public AudioClip clip;


    Color couleurOrigine;

    void Awake()
    {;
        couleurOrigine =GetComponent<Renderer>().material.color; 
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void ChangeColorHover()
    {
        this.GetComponent<Renderer>().material.color = Color.green;

    }
    //public void ChangeColorExit()
    //{
    //    this.GetComponent<Renderer>().material.color = colorbase;

    //}
    public void ChangeColorGrab()
    {
        this.GetComponent<Renderer>().material.color  = new Color(1f, 0.5f, 0f);

    }

    public void OnCollisionEnter(Collision collision)
    {
        source.PlayOneShot(clip);
    }

    public void ResetColor()
    {
        GetComponent<Renderer>().material.color = couleurOrigine;
    }


}
