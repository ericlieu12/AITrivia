using System.ComponentModel.DataAnnotations;

namespace AITrivia.DBModels
{
    public class TriviaAnswer
    {
        [Key]
        public int Id { get; set; }
        public string answerString { get; set; }

        public bool isCorrect { get; set; }
        public int TriviaQuestionId { get; set; }

    }
}
