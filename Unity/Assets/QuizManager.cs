using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuizManager : MonoBehaviour
{
    public List<GameObject> answersUIObjects = new List<GameObject>();
    public GameObject questionUIObject;

    // Class representing a question
    [System.Serializable]
    public struct Question
    {
        public string question;  // The question text
        public List<string> choices;  // List of answer choices
        public int correctChoice;  // Index of the correct answer choice
    }

    // Class representing answer data
    [System.Serializable]
    public struct AnswerData
    {
        public int questionIndex;  // Index of the answered question
        public int selectedChoice;  // Index of the selected choice
        public bool isCorrect;  // Indicates if the answer is correct
        public float timeToAnswer;  // Time taken to answer the question

        public AnswerData(int questionIndex, int selectedChoice, bool isCorrect, float timeToAnswer)
        {
            this.questionIndex = questionIndex;
            this.selectedChoice = selectedChoice;
            this.isCorrect = isCorrect;
            this.timeToAnswer = timeToAnswer;
        }
    }

    public List<Question> questions;  // List of questions
    public int currentQuestionIndex;  // Index of the current question

    private float startTime;  // Time when the current question started
    private float endTime;    // Time when the current question ended

    private int score;  // Current score
    private List<AnswerData> answers;  // List of answers with their times

    // Event triggered when a question is answered
    public event System.Action<bool> OnAnswered;

    // Event triggered when the quiz is completed
    public event System.Action<int, List<AnswerData>> OnQuizCompleted;

    private void Start()
    {
        answers = new List<AnswerData>();
        StartQuiz();
    }

    // Start the quiz
    public void StartQuiz()
    {
        currentQuestionIndex = 0;
        score = 0;
        answers.Clear();
        ShowQuestion();
    }

    // Show the current question
    private void ShowQuestion()
    {
        if (currentQuestionIndex < questions.Count)
        {
            Question currentQuestion = questions[currentQuestionIndex];
            questionUIObject.GetComponent<UnityEngine.UI.Text>().text = questions[currentQuestionIndex].question;

            // Display the question and its choices in your user interface
            for (int i = 0; i < answersUIObjects.Count; i++)
            {
                UnityEngine.UI.Text t = answersUIObjects[i].GetComponentInChildren<UnityEngine.UI.Text>();
                t.text = currentQuestion.choices[i];
            }

            startTime = Time.time;  // Start timing the current question
        }
        else
        {
            // Quiz completed
            Debug.Log("Quiz completed!");
            OnQuizCompleted?.Invoke(score, answers);
        }
    }

    // Called when the user selects an answer
    public void AnswerQuestion(int selectedChoice)
    {
        endTime = Time.time;  // Stop timing

        Question currentQuestion = questions[currentQuestionIndex];
        bool isCorrect = (selectedChoice == currentQuestion.correctChoice);

        // Store answer data
        float timeToAnswer = endTime - startTime;
        AnswerData answerData = new AnswerData(currentQuestionIndex, selectedChoice, isCorrect, timeToAnswer);
        answers.Add(answerData);

        // Update score
        if (isCorrect)
        {
            score++;
            Debug.Log("Correct!");
        }
        else
        {
            Debug.Log("Wrong!");
        }

        // Trigger the OnAnswered event
        OnAnswered?.Invoke(isCorrect);

        currentQuestionIndex++;  // Move to the next question
        ShowQuestion();
    }
}

