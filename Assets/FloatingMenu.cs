using UnityEngine;
using TMPro;

public class FloatingMenu : MonoBehaviour
{
    [SerializeField] private TextMeshPro text;                      // texte du panneau (cherché automatiquement si vide)
    [SerializeField] private float distance = 0.15f;                // 15 cm devant la main gauche
    [SerializeField] private Vector2 panelSize = new Vector2(0.2f, 0.1f);   // 20 x 10 cm
    [SerializeField] private float textOffset = 0.003f;             // texte légèrement devant le panneau

    void Start()
    {
        if (text == null)
            text = GetComponentInChildren<TextMeshPro>();

        if (text == null)
        {
            Debug.LogWarning("FloatingMenu : aucun TextMeshPro trouvé parmi les enfants !");
            return;
        }

        // Le texte doit être enfant direct du menu (et non du Quad)
        text.transform.SetParent(transform, false);

        // Réglage du texte : centré sur le panneau, un peu devant (côté visible)
        text.transform.localPosition = new Vector3(0f, 0f, -textOffset);
        text.transform.localRotation = Quaternion.identity;
        text.transform.localScale = Vector3.one;

        text.rectTransform.sizeDelta = panelSize;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.white;
        text.enableAutoSizing = true;       // la taille s'adapte au panneau
        text.fontSizeMin = 0.05f;
        text.fontSizeMax = 0.5f;
        text.text = "Menu";

        // Panneau gris foncé pour bien contraster avec le texte blanc
        Transform quad = transform.Find("Quad");
        if (quad != null)
        {
            quad.localScale = new Vector3(panelSize.x, panelSize.y, 1f);
            Renderer r = quad.GetComponent<Renderer>();
            if (r != null) r.material.color = new Color(0.2f, 0.2f, 0.2f);
        }
    }

    void LateUpdate()
    {
        // Toujours à 15 cm devant la main
        transform.localPosition = new Vector3(0f, 0f, distance);

        // Face à la caméra (le Quad est visible du côté -Z)
        if (Camera.main != null)
        {
            Vector3 directionFromCamera = transform.position - Camera.main.transform.position;
            if (directionFromCamera.sqrMagnitude > 0.0001f)
                transform.rotation = Quaternion.LookRotation(directionFromCamera);
        }

        // Affichage des positions
        if (text != null)
        {
            text.text = "Local : " + transform.localPosition.ToString("F2") + "\n"
                      + "Monde : " + transform.position.ToString("F2");
        }
    }
}