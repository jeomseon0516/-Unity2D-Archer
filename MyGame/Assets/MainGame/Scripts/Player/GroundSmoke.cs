using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundSmoke : MonoBehaviour
{
    private void Start()
    {
        TryGetComponent(out SpriteRenderer sprRen);
        sprRen.sortingOrder = (int)((transform.position.y) * 10) * -1;
    }
    // 애니메이션이 끝난 후 호출
    public void DestroySmoke()
    {
        Destroy(gameObject);
    }
}
