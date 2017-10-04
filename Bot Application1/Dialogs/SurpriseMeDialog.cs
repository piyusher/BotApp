using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace Bot_Application1.Dialogs
{
    [Serializable]
    public class SurpriseMeDialog:IDialog<object>
    {
        private bool questionAsked = false;
        private string answer = "";
        private string explanation = "";
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(this.MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            //PromptDialog.Confirm(context, AfterResetAsync, "SURE?","What?",2,PromptStyle.Inline);
            var activity = await result as Activity;
            if (questionAsked)
            {
                if (activity.Text.ToLower().Contains(answer.ToLower()))
                {
                    await context.PostAsync("Cool! You got it right!");
                    await context.PostAsync("I like talking to intelligent minds like you.");
                    context.Done<Object>(null);

                }
                else
                {
                    if(activity.Text.ToLower() == "answer" )
                    {
                        await context.PostAsync($"Well, the answer to this puzzle is - **{answer}**. {explanation}" );
                        await context.PostAsync($"I hope that would have relieved some pressure on your brain. :)");
                        context.Done<object>(null);
                        questionAsked = false;
                        answer = "";
                        explanation = "";
                    }
                    else if(activity.Text.ToLower() == "cancel" || activity.Text.ToLower() == "reset")
                    {
                        questionAsked = false;
                        answer = "";
                        explanation = "";
                        context.Done<object>(null);
                    }
                    else
                    {
                        await context.PostAsync("Uh ho! That's not correct. Try again. \n\n----\n\n You can say 'Answer' to know the answer Or say 'Cancel' Or 'Reset' to start all over again.");
                        context.Wait(this.MessageReceivedAsync);

                    }
                }
                
            }
            else
            {
                await context.PostAsync("Let's see if you can solve this!");
                JObject o = JObject.Parse(File.ReadAllText(HttpContext.Current.Server.MapPath("/Puzzles.json")));
                var lstOfPuzzles = o["Puzzles"].ToObject<List<puzzle>>();
                Random r = new Random();
                int rInt = r.Next(0, lstOfPuzzles.Count() - 1); //for ints
                await context.PostAsync(lstOfPuzzles[rInt].Q);
                questionAsked = true;
                answer = lstOfPuzzles[rInt].A;
                explanation = lstOfPuzzles[rInt].E;
                context.Wait(this.MessageReceivedAsync);
            }
           
        }

    }

    public class puzzle
    {
        public string Q;
        public string A;
        public string E;
    }
}