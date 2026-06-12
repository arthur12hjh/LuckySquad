using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "CardSO", menuName = "Scriptable Objects/CardSO")]
public class CardSO : ScriptableObject
{
    // 카드 선택용 데이터
    // 등급 : Normal < Rare < Epic < Unique < Legendary

    [SerializeField] private int gradeIndex;
    [SerializeField] private string gradeName;
    [SerializeField] private Sprite gradeImage;

    public int GradeIndex => gradeIndex;
    public string GradeName => gradeName;
    public Sprite GradeImage => gradeImage;
}
