using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using OpenWeatherMap;

namespace Bot_Application1.Dialogs
{
    [Serializable]
    public class WeatherDialog : IDialog<object>
    {
        private bool firstCall = true;
        private string cityName = "";
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            if(!firstCall)
            {
                var message = await result as Activity;
                if(message.Text.ToLower() == "cancel" || message.Text.ToLower() == "reset")
                {
                    context.Done<object>(null);
                    return;
                }

                if (cityName != "")
                {
                    if (message.Text.ToLower() == "yes")
                    {
                        var reply = await GetWeatherData(cityName);
                        if (reply.EndsWith("?"))
                        {
                            cityName = reply.TrimEnd('?');
                            await context.PostAsync("Did you mean **" + cityName + "**? Yes/No");
                        }
                        else
                        {
                            cityName = "";
                            await context.PostAsync(reply);
                        }                        
                    }
                    else if (message.Text.ToLower() == "no")
                    {
                        await context.PostAsync("Okay, let's try again. Which city?");
                        cityName = "";
                    }
                    else
                    {
                        await context.PostAsync("Sorry, i didn't get that. Did you mean **" + cityName + "** ? Yes / No \n\n----\n\n Type 'Cancel' Or 'Reset' to exit out weather module");
                    }
                }
                else
                {
                    var reply = await GetWeatherData(message.Text);
                    if (reply.EndsWith("?"))
                    {
                        cityName = reply.TrimEnd('?');
                        await context.PostAsync("Did you mean **" + cityName + "**? Yes/No");
                    }
                    else
                    {
                        await context.PostAsync(reply);
                    }
                }
                context.Wait(this.MessageReceivedAsync);

            }
            else
            {
                firstCall = false;
                await context.PostAsync("Okay! Which city?");
                context.Wait(this.MessageReceivedAsync);
            }
            
        }
        private async Task<String> GetWeatherData(String cityName)
        {
            var client = new OpenWeatherMapClient("f8a996d7160e9851b21fd328aecbde3c");
            var res = await client.Search.GetByName(cityName, MetricSystem.Imperial);
            if (res != null && res.List.Count() > 0)
            {
                var weather = res.List.FirstOrDefault();
                if(weather.City.Name.ToLower() != cityName.ToLower())
                {
                    return $"{weather.City.Name}?";
                }
                else
                {
                    return $"Currently in **{weather.City.Name}** - {weather.Temperature.Value} F \n\n **Min/Max** - {weather.Temperature.Min}/{weather.Temperature.Max} F \n\n **Wind** - {weather.Wind.Speed.Value} mph {weather.Wind.Direction.Code} \n\n **Humidity** - {weather.Humidity.Value} % \n\n----\n\n Any other city? If not, type in 'Cancel' Or 'Reset' to exit out weather module.";
                }
                

            }
            else
            {
                return "Sorry, Couldn't find that city. Please try again Or type in 'Cancel' Or 'Reset' to exit out weather module.";
            }
        }
    }
}