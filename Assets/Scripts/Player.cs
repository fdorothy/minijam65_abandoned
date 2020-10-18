using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody rb;
    public bool paused;
    public float rotationSpeed = 180.0f;
    public LayerMask terrainMask;
    public bool jumped = false;
    public bool charging = false;
    public float charge = 0.0f;
    public float maxChargeTime = 3.0f;
    public bool scoot = false;
    public float scootMultiplier = 1.0f;
    public float minCharge = 1.0f;
    public float maxVelocity;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        float dx = Input.GetAxis("Horizontal");
        float dy = Input.GetAxis("Vertical");
        float dt = Time.deltaTime;
        transform.Rotate(Vector3.up, dt * dx * rotationSpeed);
        if (charging)
        {
            charge += Time.deltaTime;
            if (charge > maxChargeTime)
            {
                charge = maxChargeTime;
            }
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            charging = true;
            charge = 0.0f;
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            charging = false;
            scoot = true;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (paused) return;
        if (scoot)
        {
            float c = Mathf.Max(charge, minCharge);
            rb.AddForce(transform.forward * c * scootMultiplier, ForceMode.VelocityChange);
            charge = 0.0f;
            scoot = false;
        }
    }

    void LookTowards(Vector3 targetDirection, float speed)
    {
        Quaternion dirQ = Quaternion.LookRotation(targetDirection);
        Quaternion slerp = Quaternion.Slerp(transform.rotation, dirQ, targetDirection.magnitude * speed * Time.deltaTime);
        rb.MoveRotation(slerp);
    }
    
    public void Reset(Vector3 startPosition)
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.MoveRotation(Quaternion.identity);
        rb.MovePosition(startPosition);
        rb.isKinematic = true;
        paused = true;
        Invoke("Release", 0.5f);
    }

    public void Release()
    {
        rb.isKinematic = false;
        paused = false;
    }
}
