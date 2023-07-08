using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Mimic.Pooling;

namespace Mimic.UI.Question {

    public class QuestionPanel : MonoBehaviour, IPoolUser {

        [SerializeField]
        protected TextPanel textPnl;

        [SerializeField]
        protected List<AnswerButtonPanel> options;

        [SerializeField]
        protected Transform answerContainer;

        [SerializeField]
        protected float delayAfterAnswering = 1.5f;

        [SerializeField]
        protected Button rereadBtn;

        [SerializeField]
        protected AnswerButtonPanel answerPrefab;

        [SerializeField]
        //Button that covers the entire screen to close the windows. Only available after the user answered the question
        private GameObject closeBtnMaskGO;

        [SerializeField]
        private bool usesPool = true;

        public enum AnswerIdentifier {
            NONE,
            NUMBER,
            CHARACTER
        }

        [SerializeField]
        private AnswerIdentifier answerIdentifier = AnswerIdentifier.NUMBER;

        private bool poolCreated = false;
        private bool startingOptionsDeleted = false;

        private short answerResult = 0; //0 for not answered, 1 for incorrect, 2 for correct

        private Answer selectedAnswer;

        private Action<Answer> correctCallback;
        private Action<Answer> incorrectCallback;

        /// <summary>
        /// Simplest question panel. Only use this method if all the panel is static (no texts are set).
        /// All other implementations of OpenQuestionPanel should call this method
        /// </summary>
        /// <param name="correctAnswerCallback">Callback after a correct answer has been clicked. First there's a "delayAfterAnswering" seconds delay</param>
        /// <param name="incorrectAnswerCallback">Callback after an incorrect answer has been clicked. First there's a "delayAfterAnswering" seconds delay</param>
        public void OpenQuestionPanel(Action<Answer> correctAnswerCallback = null, Action<Answer> incorrectAnswerCallback = null) {
            if (closeBtnMaskGO)
                closeBtnMaskGO.SetActive(false);

            if (usesPool && !poolCreated) {
                InitializePool();
            }
            options.ForEach(option => { if (option.Parent != null) option.Parent = this; });
            gameObject.SetActive(true);
            this.correctCallback = correctAnswerCallback;
            this.incorrectCallback = incorrectAnswerCallback;
        }

        /// <summary>
        /// Common question panel. It loads the answers and manages callbacks.
        /// </summary>
        /// <param name="title">Title shown at the top of the panel</param>
        /// <param name="description">Question body</param>
        /// <param name="answers">A List containing the data for each answer option</param>
        /// <param name="instantiateAnswers">True if you want the controller to clear the current options and create new ones. False if you want to reuse the same options (has to be the same amount)</param>
        /// <param name="correctAnswerCallback">Callback after a correct answer has been clicked. First there's a "delayAfterAnswering" seconds delay</param>
        /// <param name="incorrectAnswerCallback">Callback after an incorrect answer has been clicked. First there's a "delayAfterAnswering" seconds delay</param>
        public virtual void OpenQuestionPanel(string title, string description, List<Answer> answers, bool instantiateAnswers = false, 
            Action<Answer> correctAnswerCallback = null, Action<Answer> incorrectAnswerCallback = null) {

            OpenQuestionPanel(correctAnswerCallback, incorrectAnswerCallback);

            if (!String.IsNullOrEmpty(title)) {
                textPnl.Title = title;
            }
            if (!String.IsNullOrEmpty(description)) {
                textPnl.Body = description;
            }

            int answerCount = answers.Count;
            if (instantiateAnswers) {
                ClearOptions();
                AnswerButtonPanel newAnswerPnl;
                int index = 0;
                foreach (Answer answer in answers) {
                    if (usesPool) {
                        newAnswerPnl = PoolManager.Instance.GetObject<AnswerButtonPanel>(answerPrefab.name);
                        newAnswerPnl.transform.SetParent(answerContainer, false);
                    } else {
                        newAnswerPnl = Instantiate<AnswerButtonPanel>(answerPrefab, answerContainer);
                    }
                    switch (answerIdentifier) {
                        case AnswerIdentifier.NONE:
                            newAnswerPnl.SetData(this, answer);
                            break;
                        case AnswerIdentifier.NUMBER:
                            newAnswerPnl.SetData(this, index + 1, answer);
                            break;
                        case AnswerIdentifier.CHARACTER:
                            //65=A, 66=B, etc...
                            newAnswerPnl.SetData(this, (char)(65 + index), answer);
                            break;
                    }
                    index++;
                    options.Add(newAnswerPnl);
                }
            } else {
                int optionsCount = options.Count;
                for (int i = 0; i < answerCount; i++) {
                    if (i < optionsCount) {
                        options[i].SetData(this, answers[i]);
                    } else {
                        Debug.LogError("Not enough options");
                        break;
                    }
                }
            }
        }

        private void ClearOptions() {
            options.ForEach(option => {
                if (usesPool && startingOptionsDeleted)
                    PoolManager.Instance.PutBackInPool<AnswerButtonPanel>(option);
                else
                    Destroy(option.gameObject);
            });
            options.Clear();
            startingOptionsDeleted = true;
        }

        public virtual void OnQuestionAnswered(AnswerButtonPanel answerPnl) {
            if (answerPnl.IsCorrect) {
                answerPnl.SetCorrect(true);
                StartCoroutine(DelayedCallback(true, answerPnl.Answer));
            } else {
                answerPnl.SetIncorrect(true);
                foreach (AnswerButtonPanel option in options) {
                    if (option.IsCorrect) {
                        option.SetCorrect();
                    }
                }
                StartCoroutine(DelayedCallback(false, answerPnl.Answer));
            }
        }

        protected virtual IEnumerator DelayedCallback(bool success, Answer selectedAnswer) {
            if(closeBtnMaskGO!=null)
                closeBtnMaskGO.SetActive(true);
            this.selectedAnswer = selectedAnswer;
            answerResult = (short) (success ? 2 : 1);

            yield return new WaitForSecondsRealtime(delayAfterAnswering);

            answerResult = 0;
            if (success) {
                correctCallback?.Invoke(selectedAnswer);
            } else {
                incorrectCallback?.Invoke(selectedAnswer);
            }
            Close();
        }

        public void CloseAfterAnswered() {
            if (answerResult == 2) {
                correctCallback?.Invoke(selectedAnswer);
            } else if (answerResult == 1) {
                incorrectCallback?.Invoke(selectedAnswer);
            }
            Close();
        }

        protected virtual void Close() {
            gameObject.SetActive(false);
        }

        public virtual void InitializePool() {
            PoolManager.Instance.CreatePool<AnswerButtonPanel>(answerPrefab.name, answerPrefab,4);
            poolCreated = true;
        }
    }

}