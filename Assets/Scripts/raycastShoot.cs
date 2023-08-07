using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastShoot : MonoBehaviour
{
    public int gunDamage = 40;
    public float fireRate = 0.25f;
    public float weaponRange = 50f;
    public float hitForce = 100f;
    public Transform gunEnd;
    public Camera cam;
    LineRenderer lazerLine;
    Vector3 camOffset;
    float nextFire = 0f;
    // Start is called before the first frame update
    void Start()
    {
        lazerLine = GetComponent<LineRenderer>();
        lazerLine.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1") && Time.time > nextFire)
        {
            lazerLine.enabled = true;
            nextFire = Time.time + fireRate;
            StartCoroutine(shotEffect());
            Vector3 rayOrigin = cam.ViewportToWorldPoint(new Vector3(1, 1, 0));
            lazerLine.SetPosition(0, gunEnd.position);
            if (Physics.Raycast(rayOrigin, cam.transform.forward, out RaycastHit hit, weaponRange))
            {
                lazerLine.SetPosition(1, hit.point);
                if (hit.collider.gameObject.CompareTag("Slime"))
                {
                    hit.collider.gameObject.GetComponent<SlimeBehavior>().EShot(gunDamage);
                }
                if (hit.collider.gameObject.CompareTag("Turtle"))
                {
                    hit.collider.gameObject.GetComponent<TurtleBehavior>().EShot(gunDamage);
                }
            }
            else
            {
                lazerLine.SetPosition(1, rayOrigin + (cam.transform.forward * weaponRange));
            }
        }
        if (Input.GetButtonDown("Fire2"))
        {
            camOffset = Vector3.forward * 3 + Vector3.right * 0.8f - Vector3.up;
            cam.transform.localPosition += camOffset;
        }
        if (Input.GetButtonUp("Fire2"))
        {
            cam.transform.localPosition -= camOffset;
        }

    }
    private IEnumerator shotEffect()
    {
        lazerLine.enabled = true;
        yield return new WaitForSeconds(0.1f);
        lazerLine.enabled = false;
    }
}
