using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo
{ 
    private int _maxHp;
    private int _hp;
    void Update()
    {
        
    }
    public int GetMaxHp() { return _maxHp; }
    public int GetHp()    { return _hp; }

    public void SetHp(int hp) { _hp = hp; }
}
