﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using VotingApplication.Data.Context;
using VotingApplication.Data.Model;
using VotingApplication.Web.Api.Models.DBViewModels;

namespace VotingApplication.Web.Api.Controllers.API_Controllers
{
    public class PollResultsController : WebApiController
    {
        public PollResultsController() : base() { }

        public PollResultsController(IContextFactory contextFactory) : base(contextFactory) { }

        [HttpGet]
        [ResponseType(typeof(IEnumerable<VoteRequestResponseModel>))]
        public HttpResponseMessage Get(Guid pollId)
        {
            using (IVotingContext context = _contextFactory.CreateContext())
            {
                Poll poll = context
                    .Polls
                    .FirstOrDefault(s => s.UUID == pollId);

                if (poll == null)
                {
                    ThrowError(HttpStatusCode.NotFound, string.Format("Poll {0} not found", pollId));
                }

                if (Request.RequestUri != null)
                {
                    NameValueCollection queryMap = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    string lastRefreshedDate = queryMap["lastRefreshed"];

                    var clientLastUpdated = DateTime.MinValue;

                    if (lastRefreshedDate != null)
                    {
                        clientLastUpdated = UnixTimeToDateTime(long.Parse(lastRefreshedDate));
                    }

                    if (poll.LastUpdated < clientLastUpdated)
                    {
                        return new HttpResponseMessage(HttpStatusCode.NotModified);
                    }
                }

                List<Vote> votes = context
                    .Votes
                    .Include(v => v.Poll)
                    .Where(v => v.Poll.UUID == pollId)
                    .Include(v => v.Option)
                    .Include(v => v.Ballot)
                    .ToList();

                List<VoteRequestResponseModel> result = votes
                    .Select(v => VoteToModel(v, poll))
                    .ToList();

                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
        }

        private static DateTime UnixTimeToDateTime(double unixTimestamp)
        {
            var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddMilliseconds(unixTimestamp);
            return dateTime;
        }

        private VoteRequestResponseModel VoteToModel(Vote vote, Poll poll)
        {
            var model = new VoteRequestResponseModel();

            if (vote.Option != null)
            {
                model.VoterId = vote.Ballot.Id;
                model.OptionId = vote.Option.Id;
                model.OptionName = vote.Option.Name;
            }

            model.VoterName = poll.NamedVoting ? vote.Ballot.VoterName : "Anonymous User";
            model.VoteValue = vote.VoteValue;

            return model;
        }
    }
}