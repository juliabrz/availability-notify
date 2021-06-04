namespace service.Controllers
{
  using System;
  using System.Threading.Tasks;
  using AvailabilityNotify.Data;
  using AvailabilityNotify.Models;
  using AvailabilityNotify.Services;
  using Microsoft.AspNetCore.Mvc;
  using Newtonsoft.Json;

  public class EventsController : Controller
    {
        private readonly IVtexAPIService _vtexAPIService;

        public EventsController(IVtexAPIService vtexAPIService)
        {
            this._vtexAPIService = vtexAPIService ?? throw new ArgumentNullException(nameof(vtexAPIService));
        }

        public string OnAppsLinked(string account, string workspace)
        {
            return $"OnAppsLinked event detected for {account}/{workspace}";
        } 

        public void BroadcasterNotification(string account, string workspace)
        {
            Console.WriteLine($"BroadcasterNotification event detected for {account}/{workspace}");
            string bodyAsText = new System.IO.StreamReader(HttpContext.Request.Body).ReadToEndAsync().Result;
            Console.WriteLine($"[BroadcasterNotification Notification] : '{bodyAsText}'");
            BroadcastNotification notification = JsonConvert.DeserializeObject<BroadcastNotification>(bodyAsText);
            _vtexAPIService.ProcessNotification(notification);
        }

        public async Task OnAppInstalled([FromBody] AppInstalledEvent @event)
        {
            if (@event.To.Id.Contains(Constants.APP_SETTINGS))
            {
                await _vtexAPIService.VerifySchema();
                await _vtexAPIService.CreateDefaultTemplate();
            }
        }
    }
}
