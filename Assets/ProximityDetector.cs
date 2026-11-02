using UnityEngine;
using TMPro;
public class ProximityDetector : MonoBehaviour
{
    [Header("Références")]
    [SerializeField] private Transform controller;          // contrôleur droit
    [SerializeField] private GameObject[] tab;              // les cubes

    [Header("Paramètres")]
    [SerializeField] private Color proximityColor = Color.green;
    [SerializeField] private float scaleFactor = 1.2f;
    [SerializeField] private float threshold = 0.2f;        // 20 cm

    [Tooltip("Vrai : distance jusqu'à la surface du cube. Faux : jusqu'à son centre.")]
    [SerializeField] private bool useSurfaceDistance = true;

    [Header("Debug")]
    [SerializeField] private bool showDebug = false;

    // Sauvegardes des états initiaux
    private Color[] initialColors;
    private Vector3[] initialScales;
    private Renderer[] renderers;
    private Material[] mats;

    // Résultats de la mesure
    private float minDistance;
    private int closestIndex;

    [SerializeField] private float angleThreshold = 20f;    // seuil d'angle : 20°
    private float angle;
    private LineRenderer line;


    private GameObject myObject;
    private TextMeshPro myText;
    void Start()
    {
        line = gameObject.AddComponent<LineRenderer>();
        line.positionCount = 2;                     // 2 positions : contrôleur et objet
        line.startWidth = 0.005f;                   // largeur faible au départ
        line.endWidth = 0.005f;                     // et à l'arrivée
        line.useWorldSpace = true;
        line.material = new Material(Shader.Find("Sprites/Default"));   // matériau standard
        line.startColor = Color.white;              // couleur blanche
        line.endColor = Color.white;
        line.enabled = false;                       // caché au départ
        // Si le contrôleur n'est pas assigné, on tente de le trouver par son nom
        if (controller == null)
        {
            GameObject go = GameObject.Find("Right Controller");
            if (go == null) go = GameObject.Find("RightHand Controller");
            if (go != null) controller = go.transform;
            else Debug.LogWarning("ProximityDetector : Controller non assigné et introuvable !");
        }

        initialColors = new Color[tab.Length];
        initialScales = new Vector3[tab.Length];
        renderers = new Renderer[tab.Length];
        mats = new Material[tab.Length];

        for (var i = 0; i < tab.Length; i++)
        {
            if (tab[i] == null) continue;

            initialScales[i] = tab[i].transform.localScale;
            renderers[i] = tab[i].GetComponentInChildren<Renderer>();

            if (renderers[i] == null)
            {
                Debug.LogWarning("Pas de Renderer sur " + tab[i].name);
                continue;
            }

            mats[i] = renderers[i].material;   // instance propre à ce cube
            initialColors[i] = GetColor(mats[i]);
        }

        myObject = new GameObject("InfoText");                // objet vide
        myObject.transform.SetParent(controller);             // fils du contrôleur
        myObject.transform.localPosition = new Vector3(0f, 0.08f, 0.15f);  // un peu devant le contrôleur
        myObject.transform.localRotation = Quaternion.identity;

        myText = myObject.AddComponent<TextMeshPro>();        // composant TextMeshPro
        myText.fontSize = 0.6f;
        myText.alignment = TextAlignmentOptions.Center;
        myText.color = Color.white;
        myText.text = "";
    }

    void Update()
    {
        if (controller == null || tab == null || mats == null) return;
        if (Camera.main != null)
            myObject.transform.rotation = Quaternion.LookRotation(myObject.transform.position - Camera.main.transform.position);
        minDistance = float.MaxValue;
        closestIndex = -1;

        // 1) Recherche du cube le plus proche de la main
        for (var i = 0; i < tab.Length; i++)
        {
            if (tab[i] == null) continue;

            float distance;
            if (useSurfaceDistance && renderers[i] != null)
            {
                Vector3 p = renderers[i].bounds.ClosestPoint(controller.position);
                distance = Vector3.Distance(p, controller.position);
            }
            else
            {
                distance = Vector3.Distance(tab[i].transform.position, controller.position);
            }

            if (distance < minDistance)
            {
                minDistance = distance;
                closestIndex = i;
            }
        }
        bool near = false;
        bool pointed = false;
        angle = 0f;

        if (closestIndex != -1)
        {
            // vecteur de direction : main -> objet le plus proche
            Vector3 direction = tab[closestIndex].transform.position - controller.position;

            // angle entre ce vecteur et la direction forward du contrôleur
            angle = Vector3.Angle(controller.forward, direction);

            near = minDistance < threshold;       // main proche
            pointed = angle < angleThreshold;     // contrôleur orienté vers le cube
        }
        if (closestIndex != -1)
        {
            myText.enabled = true;
            myText.text = "Distance : " + minDistance.ToString("F2") + " m\n"
                        + "Angle : " + angle.ToString("F0") + "°";
        }
        else
        {
            myText.enabled = false;
        }
        if (pointed)
        {
            line.enabled = true;                                        // activer la ligne
            line.SetPosition(0, controller.position);                   // départ : contrôleur
            line.SetPosition(1, tab[closestIndex].transform.position);  // arrivée : objet pointé
        }
        else
        {
            line.enabled = false;                                       // désactiver la ligne
        }
        // 2) Couleur : verte seulement pour le plus proche, et seulement sous le seuil
        for (var i = 0; i < tab.Length; i++)
        {
            if (tab[i] == null || mats[i] == null) continue;

            if (minDistance < threshold)
                SetColor(mats[i], proximityColor);
            else
                SetColor(mats[i], initialColors[i]);
        }

        // 3) Échelle : le plus proche grossit (sous le seuil), les autres reviennent à la normale
        for (var i = 0; i < tab.Length; i++)
        {
            if (tab[i] == null) continue;

            if (i == closestIndex && near && pointed)
                tab[i].transform.localScale = initialScales[i] * scaleFactor;
            else
                tab[i].transform.localScale = initialScales[i];
        }

        if (showDebug && Time.frameCount % 30 == 0)
            Debug.Log("Cube " + closestIndex + " à " + minDistance.ToString("F2") + " m (seuil " + threshold + ")");

       
    }

    // Lecture / écriture de la couleur compatibles URP et Built-in
    private Color GetColor(Material m)
    {
        if (m.HasProperty("_BaseColor")) return m.GetColor("_BaseColor");
        if (m.HasProperty("_Color")) return m.GetColor("_Color");
        return Color.white;
    }

    private void SetColor(Material m, Color c)
    {
        if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", c);
        if (m.HasProperty("_Color")) m.SetColor("_Color", c);
    }
}