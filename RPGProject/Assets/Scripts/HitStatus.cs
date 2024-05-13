using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HitStatus : MonoBehaviour
{
    Rigidbody2D rb;
    public TMP_Text textComponent;
    [SerializeField] float timeTillFadeOut = 0.5f;
    [SerializeField] float launchDistanceMax = 5.0f;
    [SerializeField] float launchDistanceMin = 3.0f;
    [SerializeField] float speed = 5f;
    [SerializeField] float fadeOutSpeed = 1f;
    [SerializeField] float angleRange = 45f;

    Vector2 targetPos = Vector2.zero;
    bool moving;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!moving) return;

        transform.position = Vector2.Lerp(transform.position, targetPos, speed * Time.deltaTime);

        if (timeTillFadeOut > 0)
        {
            timeTillFadeOut -= Time.deltaTime;
            return;
        }

        textComponent.alpha -= fadeOutSpeed * Time.deltaTime;
        if (textComponent.alpha <= 0.01) Destroy(gameObject);
    }

    public void InputValues(string text)
    {
        textComponent.text = text;
    }
    public void Launch()
    {
        Vector2 refVector = Vector2.up;

        float angle = Random.Range(-angleRange, angleRange);

        Vector2 angleVector = Quaternion.Euler(new Vector3(0, 0, angle)) * refVector;

        targetPos = (Vector2)transform.position + angleVector * Random.Range(launchDistanceMin, launchDistanceMax);
        moving = true;
    }
}
