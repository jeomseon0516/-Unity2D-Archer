using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VibratingCamera : MonoBehaviour
{
    // 코루틴 함수 실행
    IEnumerator Start()
    {
        Camera camera = Camera.main;
        // 카메라의 진동 범위.
        Vector3 offset = new Vector3(0.05f, 0.05f, 0.0f);
        // 카메라의 진동 시간.
        float shokeTime = 0.15f;

        // 반복문이 실행되는 동안 반복적으로 호출
        while (shokeTime > 0.0f)
        {
            shokeTime -= Time.deltaTime;
            yield return null;

            // 카메라를 진동 범위 만큼 진동시킨다.
            Vector3 oldPos = camera.transform.position;
            print(oldPos);
            camera.transform.position = new Vector3(
                Random.Range(oldPos.x - offset.x, oldPos.x + offset.x),
                Random.Range(oldPos.y - offset.y, oldPos.y + offset.y),
                -10.0f);
        }

        // 반복문이 종료되면 카메라 위치를 다시 원점에 놓는다.
        // 카메라 클래스를 종료한다.
        Destroy(this.gameObject);
    }
}