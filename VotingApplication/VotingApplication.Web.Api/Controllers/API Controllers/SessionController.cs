﻿using System.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using VotingApplication.Data.Context;
using VotingApplication.Data.Model;
using System.Net.Mail;
using System.Web.Configuration;
using System.Threading.Tasks;
using System.Threading;

namespace VotingApplication.Web.Api.Controllers.API_Controllers
{
    public class SessionController : WebApiController
    {
        public SessionController() : base() { }
        public SessionController(IContextFactory contextFactory) : base(contextFactory) { }

        #region Get

        public override HttpResponseMessage Get()
        {
            return this.Request.CreateErrorResponse(HttpStatusCode.MethodNotAllowed, "Cannot use GET on this controller");
        }

        public virtual HttpResponseMessage Get(Guid id)
        {
            using (var context = _contextFactory.CreateContext())
            {
                Session matchingSession = context.Sessions.Where(s => s.UUID == id).Include(s => s.Options).FirstOrDefault();
                if (matchingSession == null)
                {
                    return this.Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format("Session {0} does not exist", id));
                }

                // Hide the manageID to prevent a GET on the poll ID from giving Poll Creator access
                matchingSession.ManageID = Guid.Empty;

                return this.Request.CreateResponse(HttpStatusCode.OK, matchingSession);
            }
        }

        #endregion

        #region Put

        public virtual HttpResponseMessage Put(object obj)
        {
            return this.Request.CreateErrorResponse(HttpStatusCode.MethodNotAllowed, "Cannot use PUT on this controller");
        }

        #endregion

        #region Post

        public virtual HttpResponseMessage Post(Session newSession)
        {
            using (var context = _contextFactory.CreateContext())
            {
                if (newSession.Name == null || newSession.Name.Length == 0)
                {
                    return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Session did not have a name");
                }

                if (newSession.Options == null)
                {
                    if (newSession.OptionSetId != 0)
                    {
                        newSession.Options = context.OptionSets.Where(os => os.Id == newSession.OptionSetId).Include(os => os.Options).FirstOrDefault().Options;
                    }
                    else
                    {
                        newSession.Options = new List<Option>();
                    }
                }

                newSession.UUID = Guid.NewGuid();
                newSession.ManageID = Guid.NewGuid();

                string creatorEmail = newSession.Email;
                List<string> invitations = newSession.Invites ?? new List<string>();


                // Do the long-running SendEmail task in a different thread, so we can return early
                Thread newThread = new Thread(new ThreadStart(() => SendEmails(creatorEmail, invitations, newSession)));
                newThread.Start();

                context.Sessions.Add(newSession);
                context.SaveChanges();

                return this.Request.CreateResponse(HttpStatusCode.OK, newSession.UUID);
            }
        }

        private void SendEmails(string creatorEmail, List<string> invitations, Session session)
        {
            string hostEmail = WebConfigurationManager.AppSettings["HostEmail"];
            string hostPassword = WebConfigurationManager.AppSettings["HostPassword"];

            if (hostEmail == null || hostPassword == null)
            {
                return;
            }

            SmtpClient client = new SmtpClient();
            client.Port = 25;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Host = "smtp.gmail.com";
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential(hostEmail, hostPassword);

            SendCreateEmail(client, creatorEmail, session);
            foreach (string inviteEmail in invitations)
            {
                SendVoteEmail(client, inviteEmail, session);
            }
        }

        private void SendCreateEmail(SmtpClient client, string targetEmailAddress, Session session)
        {
            string hostEmail = WebConfigurationManager.AppSettings["HostEmail"];

            MailMessage mail = new MailMessage(hostEmail, targetEmailAddress);

            string messageBody =
@"Your poll is now created and ready to go!

You can invite people to vote by giving them this link: http://voting-app.azurewebsites.net?poll=" + session.UUID + @"

You can administer your poll at http://voting-app.azurewebsites.net?manage=" + session.ManageID + ". Don't share this link around!";

            mail.Subject = "Your poll is ready!";
            mail.Body = messageBody;

            client.Send(mail);
        }

        private void SendVoteEmail(SmtpClient client, string targetEmailAddress, Session session)
        {
            string hostEmail = WebConfigurationManager.AppSettings["HostEmail"];

            MailMessage mail = new MailMessage(hostEmail, targetEmailAddress);

            string messageBody =
@"You've been invited by " + session.Creator + " to vote on " + session.Name + @".

Have your say at: http://voting-app.azurewebsites.net?poll=" + session.UUID + "!";

            mail.Subject = "Have your say!";
            mail.Body = messageBody;

            client.Send(mail);
        }

        public virtual HttpResponseMessage Post(Guid id, Session newSession)
        {
            using (var context = _contextFactory.CreateContext())
            {
                if (newSession == null)
                {
                    return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Session is null");
                }

                if (newSession.Name == null || newSession.Name.Length == 0)
                {
                    return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Session does not have a name");
                }

                if (newSession.Options == null)
                {
                    List<Option> options = new List<Option>();
                    if (newSession.OptionSetId != 0)
                    {
                        options = context.OptionSets.Where(os => os.Id == newSession.OptionSetId).Include(os => os.Options).FirstOrDefault().Options;
                    }

                    newSession.Options = options;
                }

                Session matchingSession = context.Sessions.Where(s => s.UUID == id).FirstOrDefault();
                if (matchingSession != null)
                {
                    context.Sessions.Remove(matchingSession);
                }

                context.Sessions.Add(newSession);
                context.SaveChanges();

                return this.Request.CreateResponse(HttpStatusCode.OK, newSession.UUID);
            }
        }

        #endregion
    }
}