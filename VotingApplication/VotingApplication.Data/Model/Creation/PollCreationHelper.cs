﻿
using System;
using System.Collections.Generic;

namespace VotingApplication.Data.Model.Creation
{
    public static class PollCreationHelper
    {
        public static Poll Create()
        {
            return new Poll()
            {
                UUID = Guid.NewGuid(),
                ManageId = Guid.NewGuid(),

                PollType = PollType.Basic,
                Options = new List<Option>(),
                MaxPoints = null,
                MaxPerVote = null,
                InviteOnly = false,
                NamedVoting = false,
                ExpiryDateUtc = null,
                OptionAdding = false,

                CreatedDateUtc = DateTime.UtcNow,
                LastUpdatedUtc = DateTime.UtcNow
            };
        }

    }
}
