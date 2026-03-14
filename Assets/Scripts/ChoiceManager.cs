using UnityEngine;
using TMPro;

public class ChoiceManager : MonoBehaviour
{
    [SerializeField] private GameObject choicePanel;

    [SerializeField] private TMP_Text questionText;
    [SerializeField] private TMP_Text[] optionTexts; // size = 3

    [SerializeField] private QuestionChoiceData[] questions;

    private int correctButtonIndex;

    public void ShowRandomQuestion()
    {
        choicePanel.SetActive(true);
        Time.timeScale = 0f;

        // 1?? Pick random question
        QuestionChoiceData q = questions[Random.Range(0, questions.Length)];

        questionText.text = q.Question;

        // 2?? Store options
        string[] options =
        {
            q.option1,
            q.option2,
            q.option3
        };

        int correctOriginalIndex = q.correctOption;

        // 3?? Create index shuffle
        int[] order = { 0, 1, 2 };

        for (int i = 0; i < order.Length; i++)
        {
            int rand = Random.Range(i, order.Length);

            int temp = order[i];
            order[i] = order[rand];
            order[rand] = temp;
        }

        // 4?? Assign shuffled options to UI
        for (int i = 0; i < 3; i++)
        {
            optionTexts[i].text = options[order[i]];

            if (order[i] == correctOriginalIndex)
                correctButtonIndex = i;
        }
    }

    public void SelectOption(int index)
    {
        Time.timeScale = 1f;
        choicePanel.SetActive(false);

        if (index == correctButtonIndex)
        {
            Debug.Log("Correct");
            ContinueGame();
        }
        else
        {
            Debug.Log("Wrong");
            GameOver();
        }
    }

    void ContinueGame()
    {
        // continue gameplay
    }

    void GameOver()
    {
        // trigger game over
    }
}
