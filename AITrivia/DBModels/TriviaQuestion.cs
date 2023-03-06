using System.ComponentModel.DataAnnotations;

namespace AITrivia.DBModels
{
    public class TriviaQuestion
    {
        [Key]
        public int Id { get; set; }
        public string questionString { get; set; }
        public virtual List<TriviaAnswer> answers { get; set; }

        public int LobbyId { get; set; }
    }
}
