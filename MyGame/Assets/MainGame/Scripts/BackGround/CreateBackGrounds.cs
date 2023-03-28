using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateBackGrounds : MonoBehaviour
{
    List<BackGroundsController> _backGrounds = new List<BackGroundsController>();
    void Awake()
    {
        for (int i = 0; i < transform.childCount; ++i)
            _backGrounds.Add(transform.GetChild(i).GetComponent<BackGroundsController>());

        for (int i = 0; i < _backGrounds.Count; ++i)
        {
            _backGrounds[i].SetSpeed(SetBackGroundSpeed(_backGrounds[i], i));

             if (Default.GetIntParseString(_backGrounds[i].name) == _backGrounds.Count) 
            continue;

            var clone = Instantiate(_backGrounds[i], transform);
            clone.SetSpeed(SetBackGroundSpeed(_backGrounds[i], i));
        }
    }
    void Start()
    {
        for (int i = 0; i < _backGrounds.Count; i++)
        {
            _backGrounds[i].transform.position = _backGrounds[i].transform.position +
                                   new Vector3(_backGrounds[i].GetSizeX(), 0.0f, 0.0f);
        }
    }

    private float SetBackGroundSpeed(BackGroundsController backGround, int count)
    {
        float speed = 0.0f;
        switch (count)
        {
            case 0:
                speed = 0.25f;
                break;
            case 1:
                speed = 0.125f;
                break;
            case 2:
                speed = 0;
                break;
            case 3:
                speed = -0.5f;
                break;
            case 4:
                speed = -0.75f;
                break;
            case 5:
                speed = -0.85f;
                break;
            case 6:
                speed = -0.9f;
                break;
            case 7:
                speed = -0.95f;
                break;
            default:
                speed = -1;
                break;
        }
        return speed;
    }
}
