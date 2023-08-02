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
                Debug.Log("hit " + hit.collider.gameObject.name);
                if (hit.collider.gameObject.CompareTag("Enemy"))
                {
                    hit.collider.gameObject.GetComponent<EnemyBehavior>().EShot(gunDamage);
                }
            }
            else
            {
                lazerLine.SetPosition(1, rayOrigin + (cam.transform.forward * weaponRange));
            }
        }
        if (Input.GetButtonDown("Fire2"))
        {
            cam.transform.position += cam.transform.forward * 3;
            cam.transform.position += cam.transform.right;
            cam.transform.position -= cam.transform.up;
        }
        if (Input.GetButtonUp("Fire2"))
        {
            Debug.Log(cam.transform.forward);
            Debug.Log(cam.transform.position);
            cam.transform.position -= cam.transform.forward * 3;
            cam.transform.position -= cam.transform.right;
            cam.transform.position += cam.transform.up;
        }

    }
    private IEnumerator shotEffect()
    {
        lazerLine.enabled = true;
        yield return new WaitForSeconds(0.1f);
        lazerLine.enabled = false;
    }
}
