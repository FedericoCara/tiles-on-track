
namespace Mimic.UI.Question {

    public class Answer {

        protected string description;
        public string Description { get { return description; } }

        protected bool isCorrect = false;
        public bool IsCorrect { get { return isCorrect; } }

        public Answer(string description) {
            this.description = description;
        }

        public Answer(string description, bool isCorrect) {
            this.description = description;
            this.isCorrect = isCorrect;
        }
    }

}
