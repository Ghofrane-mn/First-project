using UnityEngine;

public class PatrolMovement : MonoBehaviour
{
    public enum MovementType { MoveTowards, Lerp, Smooth, Custom }

    [Header("Bornes")]
    [SerializeField] private Transform borneA;
    [SerializeField] private Transform borneB;

    [Header("Déplacement")]
    [SerializeField] private MovementType movementType = MovementType.MoveTowards;
    [Tooltip("Vitesse en mètres par seconde")]
    [SerializeField] private float speed = 2f;
    [Tooltip("Distance en dessous de laquelle on considère la borne atteinte (MoveTowards)")]
    [SerializeField] private float arrivalThreshold = 0.01f;

    [Header("Mode Custom")]
    [SerializeField] private AnimationCurve customCurve = AnimationCurve.Linear(0, 0, 1, 1);

    // MoveTowards : on mémorise la cible courante
    private bool goingToB = true;

    // Lerp / Smooth / Custom : progression cumulée (évite les sauts si la distance change)
    private float progress = 0f;

    private void Start()
    {
        if (borneA != null)
            transform.position = borneA.position; // part de la 1ère borne
    }

    private void Update()
    {
        if (borneA == null || borneB == null) return;

        switch (movementType)
        {
            case MovementType.MoveTowards:
                MoveTowardsMovement();
                break;
            case MovementType.Lerp:
                LerpMovement();
                break;
            case MovementType.Smooth:
                SmoothMovement();
                break;
            case MovementType.Custom:
                CustomMovement();
                break;
        }
    }

    // --- Mode 1 : MoveTowards -------------------------------------------
    private void MoveTowardsMovement()
    {
        Vector3 target = goingToB ? borneB.position : borneA.position;

        transform.position = Vector3.MoveTowards(
            transform.position, target, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target) < arrivalThreshold)
            goingToB = !goingToB; // on repart vers l'autre borne
    }

    // --- Mode 2 : Lerp ---------------------------------------------------
    private void LerpMovement()
    {
        float t = GetPingPong();
        transform.position = Vector3.Lerp(borneA.position, borneB.position, t);
    }

    // --- Mode 3 : Smooth -------------------------------------------------
    private void SmoothMovement()
    {
        float t = Mathf.SmoothStep(0f, 1f, GetPingPong());
        transform.position = Vector3.Lerp(borneA.position, borneB.position, t);
    }

    // --- Bonus : Custom (AnimationCurve) ---------------------------------
    private void CustomMovement()
    {
        float t = customCurve.Evaluate(GetPingPong());
        transform.position = Vector3.LerpUnclamped(borneA.position, borneB.position, t);
    }

    // Retourne un t entre 0 et 1 qui fait des allers-retours.
    // Bonus : la durée d'un trajet = distance / vitesse, donc la vitesse est en m/s.
    private float GetPingPong()
    {
        float distance = Vector3.Distance(borneA.position, borneB.position);
        if (distance < 0.0001f) return 0f;

        float duration = distance / speed;      // temps pour parcourir A -> B
        progress += Time.deltaTime / duration;  // on accumule pour éviter les sauts
        return Mathf.PingPong(progress, 1f);
    }
}