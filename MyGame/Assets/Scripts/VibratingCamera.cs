using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VibratingCamera : MonoBehaviour
{
    IEnumerator Start()
    {
        Camera camera = Camera.main;
        Vector3 offset = new Vector3(0.05f, 0.05f, 0.0f);
        float shokeTime = 0.15f;

        while (shokeTime > 0.0f)
        {
            shokeTime -= Time.deltaTime;
            yield return null;

            Vector3 oldPos = camera.transform.position;

            camera.transform.position = new Vector3(
                Random.Range(oldPos.x - offset.x, oldPos.x + offset.x),
                Random.Range(oldPos.y - offset.y, oldPos.y + offset.y),
                -10.0f);
        }

        Destroy(this.gameObject);
    }
}