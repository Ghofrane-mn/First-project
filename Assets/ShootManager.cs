using UnityEngine;
using UnityEngine.XR;

public class ShootManager : MonoBehaviour
{
    [Header("Visée")]
    [SerializeField] private Transform controller;      // Transform de la manette droite
    [SerializeField] private float angleTolerance = 5f; // tolérance en degrés

    [Header("Feedbacks")]
    [SerializeField] private GameObject particlePrefab;
    [SerializeField] private AudioClip shootSound;

    private GameObject[] targets;
    private bool triggerWasPressed = false;

    private void Start()
    {
        targets = GameObject.FindGameObjectsWithTag("Target");
        Debug.Log("Cibles trouvées : " + targets.Length);
    }

    private void Update()
    {
        var devices = new System.Collections.Generic.List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(
            InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller, devices);

        if (devices.Count == 0)
        {
            Debug.Log("Aucune manette droite");
            return;
        }
        InputDevice rightHand = devices[0];

        if (!rightHand.isValid)
        {
            Debug.Log("Manette droite NON détectée");
            return;
        }

        rightHand.TryGetFeatureValue(CommonUsages.triggerButton, out bool triggerPressed);
        rightHand.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue);

        if (triggerValue > 0.1f)
            Debug.Log("Valeur gâchette : " + triggerValue);

        if (triggerPressed && !triggerWasPressed)
        {
            Debug.Log("Gâchette appuyée");
            Shoot();
        }

        triggerWasPressed = triggerPressed;
    }
    private void Shoot()
    {
        GameObject bestTarget = null;
        float bestAngle = angleTolerance;

        foreach (GameObject target in targets)
        {
            if (target == null || !target.activeInHierarchy) continue;

            Vector3 direction = target.transform.position - controller.position;
            float angle = Vector3.Angle(controller.forward, direction);
            Debug.Log("Angle vers " + target.name + " : " + angle);

            if (angle < bestAngle)
            {
                bestAngle = angle;
                bestTarget = target;
            }
        }

        if (bestTarget != null)
            Hit(bestTarget);
        else
            Debug.Log("Aucune cible dans la tolérance");
    }
    private void Hit(GameObject target)
    {
        if (particlePrefab != null)
            Instantiate(particlePrefab, target.transform.position, Quaternion.identity);

        AudioSource audioSource = target.GetComponent<AudioSource>();
        if (audioSource != null && shootSound != null)
            audioSource.PlayOneShot(shootSound);

        target.GetComponent<Renderer>().enabled = false;
        target.GetComponent<Collider>().enabled = false;
        Destroy(target, shootSound != null ? shootSound.length : 0f);
    }
}