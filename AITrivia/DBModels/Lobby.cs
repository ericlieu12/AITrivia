﻿using System.ComponentModel.DataAnnotations;

namespace AITrivia.DBModels
{
    public class Lobby
    {
        [Key]
        public int Id { get; set; }
        public string UrlString { get; set; }

        public bool isStarted { get; set; }
        public bool isDone { get; set; }

        public int questionNumber { get; set; }

        public int answersComplete { get; set; }

        [Timestamp]
        public byte[] Version { get; set; }
        public virtual List<TriviaQuestion> triviaQuestions { get; set; }
        public virtual List<User> users { get; set; }

    }
}
