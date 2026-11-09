using UnityEngine;

public class onHover : MonoBehaviour
{
    private float duree = 30f;
    private Color colorBase;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
         colorBase = gameObject.GetComponent<Renderer>().material.color;
    }

    // Update is called once per frame
    void Update()
    {

        
        
    }


   
 
    public  void ChangerCouleur()
    {
        GameObject bestTarget = this.gameObject;
        float tempsEcoule = 0f;


        while (tempsEcoule < duree)
        {
            tempsEcoule += Time.deltaTime;
            float progression = tempsEcoule / duree;

            bestTarget.GetComponent<Renderer>().material.color = Color.Lerp(bestTarget.GetComponent<Renderer>().material.color, Color.red, progression);

        }
        bestTarget.GetComponent<Renderer>().material.color = Color.red;
    }


    public void ChangerCouleurExit()
    {
        gameObject.GetComponent<Renderer>().material.color = colorBase;

    }
}
