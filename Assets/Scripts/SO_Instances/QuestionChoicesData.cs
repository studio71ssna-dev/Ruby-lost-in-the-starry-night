using UnityEngine;

[CreateAssetMenu(fileName = "QuestionChoice", menuName = "Choice Game/Question Choice")]
public class QuestionChoiceData : ScriptableObject
{
    [TextArea]
    public string Question;

    [TextArea]
    public string option1;

    [TextArea]
    public string option2;

    [TextArea]
    public string option3;

    public int correctOption = 1;
}
