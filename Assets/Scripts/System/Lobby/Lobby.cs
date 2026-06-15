using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Lobby : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI stageText;
    [SerializeField] private Button prevStageButton;
    [SerializeField] private Button nextStageButton;

    private uint stageIndex = 0;
    private const uint maxStageIndex = 5;

    private void Awake()
    {
        UpdateStageUI();
    }

    public void OnClickPrevStage()
    {
        if (stageIndex == 0)
            return;

        --stageIndex;
        UpdateStageUI();
    }

    public void OnClickNextStage()
    {
        if (stageIndex == maxStageIndex)
            return;

        ++stageIndex;
        UpdateStageUI();
    }

    public void Start_Stage()
    {
        GameManager.Instance.SetStage(stageIndex);
        GameManager.Instance.ChangeScene(Enums.SceneType.Stage);
        SceneManager.LoadScene("Loading");
    }

    private void UpdateStageUI()
    {
        stageText.text = $"STAGE {(stageIndex + 1)}" ;

        prevStageButton.interactable = stageIndex > 0;
        nextStageButton.interactable = stageIndex < maxStageIndex;
    }

}
