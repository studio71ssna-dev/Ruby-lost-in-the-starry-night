using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class QuizManager : MonoBehaviour
{
    [SerializeField] private GameObject choicePanel;

    [SerializeField] private TMP_Text questionText;
    [SerializeField] private TMP_Text[] optionTexts; // size = 3
    GameObject Wolf;
    [SerializeField] private QuestionChoiceData[] questions;
    public UnityEvent WrongChoice;

    private int correctButtonIndex;

    public void ShowRandomQuestion()
    {
        choicePanel.SetActive(true);
        Time.timeScale = 0f;

        QuestionChoiceData q = questions[Random.Range(0, questions.Length)];

        questionText.text = q.Question;

        string[] options =
        {
            q.option1,
            q.option2,
            q.option3
        };

        int correctOriginalIndex = q.correctOption;

        int[] order = { 0, 1, 2 };

        for (int i = 0; i < order.Length; i++)
        {
            int rand = Random.Range(i, order.Length);

            int temp = order[i];
            order[i] = order[rand];
            order[rand] = temp;
        }

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

    private void Update()
    {
        Wolf=GameObject.FindGameObjectWithTag("Wolf");
    }

    void ContinueGame()
    {
        Wolf.SetActive(false);
        InputHandler.Instance.SwitchActionMap("Player_Movement");
    }

    void GameOver()
    {
        WrongChoice.Invoke();
    }
}
