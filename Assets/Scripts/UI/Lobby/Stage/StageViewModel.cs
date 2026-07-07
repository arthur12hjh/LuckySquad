using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageViewModel : BaseViewModel
{
    [SerializeField] private StageRef[] stages;

    private uint stageIndex = 1;
    private const uint maxStageIndex = 6;

    public event Action<uint> OnStageChanged;

    public override void Initialize() 
    {
    }

    public override void Release() 
    {
    }

    public void PrevStage()
    {
        if (stageIndex == 1)
            return;

        --stageIndex;
        OnStageChanged?.Invoke(stageIndex);
    }

    public void NextStage()
    {
        if (stageIndex == maxStageIndex)
            return;

        ++stageIndex;
        OnStageChanged?.Invoke(stageIndex);
    }

    public void StartStage()
    {
        //GameManager.Instance.SetStage(stageIndex);
        GameManager.Instance.SetStage(1);
        GameManager.Instance.ChangeScene(Enums.SceneType.Stage);
        SceneManager.LoadScene("Loading");
    }
}
