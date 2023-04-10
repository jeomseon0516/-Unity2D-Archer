using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonTemplate<GameManager>
{
    protected override void Init() {}
    
    public void StartGame() 
    {
        EnemyManager.GetInstance().SetIsMakeEnemy(true);
        PlayerManager.GetInstance().SetActivePlayer(true);
        UIManager.GetInstance().SetActiveUI("MainMenu", false);
        UIManager.GetInstance().SetActiveUI("PlayerHpBar", true);
    }
    public void ExitGame()
    {
        UIManager.GetInstance().SetActiveUI("MainMenu", true);
        EnemyManager.GetInstance().SetIsMakeEnemy(false);
        EnemyManager.GetInstance().AllRemoveEnemy();
        PlayerManager.GetInstance().SetActivePlayer(false);
    }
    private GameManager() {}
}
