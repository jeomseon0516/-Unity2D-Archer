using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VibratingCamera : MonoBehaviour
{
    // 코루???�수 ?�행
    IEnumerator Start()
    {
        Camera camera = Camera.main;
        // 카메?�의 진동 범위.
        Vector3 offset = new Vector3(0.05f, 0.05f, 0.0f);
        // 카메?�의 진동 ?�간.
        float shokeTime = 0.15f;

        // 반복문이 ?�행?�는 ?�안 반복?�으�??�출
        while (shokeTime > 0.0f)
        {
            shokeTime -= Time.deltaTime;
            yield return null;

            // 카메?��? 진동 범위 만큼 진동?�킨??
            Vector3 oldPos = camera.transform.position;
            print(oldPos);
            camera.transform.position = new Vector3(
                Random.Range(oldPos.x - offset.x, oldPos.x + offset.x),
                Random.Range(oldPos.y - offset.y, oldPos.y + offset.y),
                -10.0f);
        }

        // 반복문이 종료?�면 카메???�치�??�시 ?�점???�는??
        // 카메???�래?��? 종료?�다.
        Destroy(this.gameObject);
    }
}