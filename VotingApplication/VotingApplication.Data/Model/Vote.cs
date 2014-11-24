﻿using System;
namespace VotingApplication.Data.Model
{
    public class Vote
    {
        public long Id { get; set; }

        public long OptionId { get; set; }
        public Option Option { get; set; }

        public int PollValue { get; set; }

        public long UserId { get; set; }
        public User User { get; set; }

        public Guid SessionId { get; set; }
        public Session Session { get; set; }
    }
}
