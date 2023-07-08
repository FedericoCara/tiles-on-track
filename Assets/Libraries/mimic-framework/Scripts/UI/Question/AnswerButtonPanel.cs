using UnityEngine;
using UnityEngine.UI;

using Mimic.Pooling;

namespace Mimic.UI.Question {

    public class AnswerButtonPanel : MonoBehaviour, IPoolable {

        [SerializeField]
        protected Text questionNumberTxt;

        [SerializeField]
        protected Text questionTxt;

        [SerializeField]
        protected Image buttonBackgroundImg;

        [SerializeField]
        protected Sprite answerSprite, answerRightSprite, answerIncorrectSprite;

        [SerializeField]
        protected Color commonTxtColor = Color.black;
                
        [SerializeField]
        protected Color rightAnswerTxtColor = Color.green;

        [SerializeField]
        protected Color wrongAnswerTxtColor = Color.red;

        [SerializeField]
        protected Button button;

        public string PoolKey { get; set; }
        private Answer answer;
        public Answer Answer { get { return answer; } }

        public bool IsCorrect { get { return answer.IsCorrect; } }

        protected QuestionPanel parent;
        public QuestionPanel Parent {
            get { return parent; }
            set {
                if (parent != value && parent != null) {
                    Debug.LogWarning("Changing Question Parent on an existing anwser is not an expected behaviour");
                    button.onClick.RemoveAllListeners();
                }
                parent = value;
                button.onClick.AddListener(() => { parent.OnQuestionAnswered(this); });
            }
        }

        public virtual void SetData(QuestionPanel parent, Answer answer) {
            Parent = parent;

            this.answer = answer;
            questionTxt.text = answer.Description;

            buttonBackgroundImg.sprite = answerSprite;
            questionNumberTxt.color = commonTxtColor;
            questionTxt.color = commonTxtColor;
            button.interactable = true;
        }

        public void SetData(QuestionPanel parent, int number, Answer answer) {
            questionNumberTxt.text = number.ToString();
            SetData(parent, answer);
        }

        public void SetData(QuestionPanel parent, char character, Answer answer) {
            questionNumberTxt.text = character.ToString();
            SetData(parent, answer);
        }

        public virtual void SetCorrect(bool playSound = false) {
            buttonBackgroundImg.sprite = answerRightSprite;
            questionNumberTxt.color = rightAnswerTxtColor;
            questionTxt.color = rightAnswerTxtColor;
            button.interactable = false;
        }

        public virtual void SetIncorrect(bool playSound = false) {
            buttonBackgroundImg.sprite = answerIncorrectSprite;
            questionNumberTxt.color = wrongAnswerTxtColor;
            questionTxt.color = wrongAnswerTxtColor;
            button.interactable = false;
        }

        public void Initialize() {
            transform.localScale = Vector3.one;
            transform.rotation = Quaternion.identity;
            button.interactable = true;
        }

        public void OnPutBackInPool() {
            //Nothing
        }

    }

}