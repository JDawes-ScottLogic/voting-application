﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VotingApplication.Data.Model
{
    public class Ballot
    {
        public Ballot()
        {
            Votes = new List<Vote>();
        }

        public long Id { get; set; }

        [Required]
        public Guid ManageGuid { get; set; }

        public Guid TokenGuid { get; set; }
        public string Email { get; set; }
        public String VoterName { get; set; }
        public List<Vote> Votes { get; set; }
    }
}
