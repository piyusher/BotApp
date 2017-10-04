using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Configuration;

namespace Bot_Application1
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                await Conversation.SendAsync(activity, () => new Dialogs.RootDialog());
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
                if (message.MembersAdded != null && message.MembersAdded.Count > 0)
                {
                    foreach (var newMember in message.MembersAdded)
                    {
                        if (newMember.Id != message.Recipient.Id)
                        {
                            var reply = message.CreateReply();
                            reply.Text = $"Welcome! My name is PKBot and i am programmed by Piyush Khanna.";
                            MicrosoftAppCredentials cred = new MicrosoftAppCredentials();
                            cred.MicrosoftAppId = ConfigurationManager.AppSettings["MicrosoftAppId"];
                            cred.MicrosoftAppPassword = ConfigurationManager.AppSettings["MicrosoftAppPassword"];

                            var connector = new ConnectorClient(new Uri(message.ServiceUrl.ToString()),cred);
                            connector.Conversations.ReplyToActivityAsync(reply);
                        }
                    }
                }
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}