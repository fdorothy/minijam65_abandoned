using UnityEngine;

public class Pickup : MonoBehaviour
{
    float width;
    float t = 0.0f, t0;
    float speed;
    Vector3 initialPosition;
    Vector3 dir;

    private void OnTriggerEnter(Collider other)
    {
        Game.singleton.coins += 1;
        Destroy(gameObject);
    }
}
