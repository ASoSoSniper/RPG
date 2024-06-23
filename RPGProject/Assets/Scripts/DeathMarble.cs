using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class DeathMarble : MonoBehaviour
{
    [SerializeField] float launchStrength = 5f;
    [SerializeField] float torqueStrength = 5f;
    [SerializeField] float angleRange = 45f;

    [SerializeField] float lifeTime = 2f;

    // Start is called before the first frame update
    void Start()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb)
        {
            Vector2 refVector = Vector2.up;
            float angle = Random.Range(-angleRange, angleRange);

            Vector2 angleVector = Quaternion.Euler(new Vector3(0, 0, angle)) * refVector;

            rb.AddForce(angleVector * launchStrength, ForceMode2D.Impulse);
            rb.AddTorque(Random.Range(0, 2) == 0 ? torqueStrength : -torqueStrength, ForceMode2D.Impulse);
        }
    }

    private void Update()
    {
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0 )
        {
            Destroy(gameObject);
        }
    }
}
