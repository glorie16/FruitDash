using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class PlayerShooter : MonoBehaviour
{
    [Header("Shooting")]
    [SerializeField] GameObject arrowPrefab;
    [SerializeField] Transform firePoint;
    [SerializeField] float fireRate = 0.3f;

    SpriteRenderer spriteRenderer;
    float nextFireTime;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame 
            && !EventSystem.current.IsPointerOverGameObject() 
            && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

   void Shoot()
{
    if (arrowPrefab == null)
    {
        Debug.LogError("arrowPrefab is not assigned!");
        return;
    }
    if (firePoint == null)
    {
        Debug.LogError("firePoint is not assigned!");
        return;
    }

    GameObject arrow = Instantiate(arrowPrefab, firePoint.position, Quaternion.identity);
    AudioManager.Instance.PlayShoot();

    Arrow arrowScript = arrow.GetComponent<Arrow>();
    if (arrowScript != null)
    {
        float direction = spriteRenderer.flipX ? -1f : 1f;
        arrowScript.SetDirection(direction);
    }
    else
    {
        Debug.LogError("Arrow prefab is missing the Arrow script!");
    }
}
}