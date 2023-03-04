using System.ComponentModel.DataAnnotations;

namespace AITrivia.DBModels
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string ConnectionID { get; set; }
        public bool IsLeader { get; set; }
        public int LobbyId { get; set; }

    }
}
