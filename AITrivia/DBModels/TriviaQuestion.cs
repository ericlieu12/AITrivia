using System.ComponentModel.DataAnnotations;

namespace AITrivia.DBModels
{
    public class TriviaQuestion
    {
        [Key]
        public int Id { get; set; }
        public string questionString { get; set; }
        public TriviaAnswer correctAnswer { get; set; }
        public virtual List<User> users { get; set; }

        public int LobbyId { get; set; }
    }
}
