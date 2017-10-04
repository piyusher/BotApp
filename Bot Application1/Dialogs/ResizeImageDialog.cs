using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Collections.Generic;
using OpenWeatherMap;

namespace Bot_Application1.Dialogs
{
    [Serializable]
    public class ResizeImageDialog : IDialog<object>
    {
        private int retryCount = 0;
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(this.MessageReceivedAsync);
            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var act = await result as Activity;
            if(act.Attachments.Count == 0)
            { 
                if(retryCount == 0)
                {
                    retryCount++;
                   
                    await context.PostAsync("Go ahead and send me the image or type 'Cancel' to reset the conversation!");
                    context.Wait(this.MessageReceivedAsync);
                    
                }
                else
                {
                    if (act.Text.ToLower() != "cancel")
                        await context.PostAsync("Sorry, Couldn't get that! Send me the image you want to resize or type 'Cancel' to reset the conversation!");
                    else
                        context.Done<object>(null);
                }
                
            }
            else
            {
                Activity message = await result as Activity;
                await context.PostAsync("Got your Image!");
                context.Done<object>(null);
            } 
        }
    }
}