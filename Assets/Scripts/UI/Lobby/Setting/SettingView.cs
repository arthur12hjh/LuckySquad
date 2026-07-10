using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingView : BaseView
{
    [SerializeField] private Toggle BGMToggle;
    [SerializeField] private Toggle SFXToggle;
    [SerializeField] private Scrollbar BGMScrollbar;
    [SerializeField] private Scrollbar SFXScrollbar;
    [SerializeField] private Sprite[] BGMToggleSprite;
    [SerializeField] private Sprite[] SFXToggleSprite;

    private SettingViewModel _viewModel;
    private float preBGMSound;
    private float preSFXSound;
    private const float minSound = 0.1f;

    public override void Bind(BaseViewModel baseViewModel)
    {
        if (null != _viewModel)
            return;

        if (baseViewModel is SettingViewModel stageView)
        {
            _viewModel = stageView;
            _viewModel.Initialize();

            // TODO : SettingViewModel을 통해 SoundManager 연결

            BGMToggle.onValueChanged.AddListener(OnBGMToggleChanged);
            SFXToggle.onValueChanged.AddListener(OnSFXToggleChanged);

            BGMScrollbar.onValueChanged.AddListener((float sound) => { BGMToggle.isOn = sound > 0f; });
            SFXScrollbar.onValueChanged.AddListener((float sound) => { SFXToggle.isOn = sound > 0f; });
        }
        else
        {
            Debug.Log("잘못된 SettingViewModel Binding");
            return;
        }

        preBGMSound = BGMScrollbar.value;
        preSFXSound = SFXScrollbar.value;
    }

    private void OnBGMToggleChanged(bool isOn)
    {
        if (BGMToggle.targetGraphic is Image image)
        {
            if (isOn)
            {
                image.sprite = BGMToggleSprite[0];
                BGMScrollbar.value = preBGMSound;
            }
            else
            {
                image.sprite = BGMToggleSprite[1];
                preBGMSound = Mathf.Max(minSound, BGMScrollbar.value);
                BGMScrollbar.value = 0f;
            }
        }
    }

    private void OnSFXToggleChanged(bool isOn)
    {
        if (SFXToggle.targetGraphic is Image image)
        {
            if (isOn)
            {
                image.sprite = SFXToggleSprite[0];
                SFXScrollbar.value = preSFXSound;
            }
            else
            {
                image.sprite = SFXToggleSprite[1];
                preSFXSound = Mathf.Max(minSound, SFXScrollbar.value);
                SFXScrollbar.value = 0f;
            }
        }
    }
}
