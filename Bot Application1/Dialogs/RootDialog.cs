using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Collections.Generic;
using System.Threading;

namespace Bot_Application1.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(this.MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            //PromptDialog.Confirm(context, AfterResetAsync, "SURE?","What?",2,PromptStyle.Inline);
            var activity = await result as Activity;
            string txt = activity.Text.ToLower() + " ";
            if (txt.StartsWith("hi ") || txt.StartsWith("Hello ") || txt.StartsWith("hey "))
            {
                await context.PostAsync("Hi! My name is PK Bot and I am created by Piyush Khanna using BotFramework! I can do following things. What would you like to do?");
                ShowIntroPrompt(context);
                
            }
            if (activity.Text.ToLower().Contains("about"))
            {
                await context.Forward(new ResizeImageDialog(), this.MessageReceivedAsync, activity);
            }
        }

        private void ShowIntroPrompt(IDialogContext context)
        {
            var opt = new PromptOptions<String>("Choose from following -", "Didn't get that, please try again", "You are tough! I give up!", new string[] {"Puzzle", "Weather" });
            PromptDialog.Choice(context, AfterIntroResetAsync, opt);
        }

        public async Task AfterIntroResetAsync(IDialogContext context, IAwaitable<string> argument)
        {
            var confirm = await argument;
            if (confirm.ToLower().StartsWith("weather"))
            {
                await context.Forward(new WeatherDialog(), AfterAllDoneAsync, context.Activity,CancellationToken.None);
            }
            else if (confirm.ToLower().StartsWith("puzzle"))
            {
                await context.Forward(new SurpriseMeDialog(), AfterAllDoneAsync, context.Activity, CancellationToken.None);
            }
            else
            {
                context.Wait(MessageReceivedAsync);
            }
        }
        public async Task AfterAllDoneAsync(IDialogContext context, IAwaitable<object> argument)
        {
            await context.PostAsync("Let's start over!");
            ShowIntroPrompt(context);
        }
    }
}