using UnityEngine;

[CreateAssetMenu(fileName = "QuestionChoice", menuName = "Choice Game/Question Choice")]
public class QuestionChoiceData : ScriptableObject
{
    public string Question;

    [TextArea]
    public string option1;

    [TextArea]
    public string option2;

    [TextArea]
    public string option3;

    [Range(0, 2)]
    public int correctOption;
}
