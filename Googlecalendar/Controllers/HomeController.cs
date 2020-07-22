#region Copyright Syncfusion Inc. 2001-2018.
// Copyright Syncfusion Inc. 2001-2018. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Googlecalendar.Models;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.IO;
using System.Threading;

namespace Googlecalendar.Controllers
{
    public class HomeController : Controller
    {
        static string[] Scopes = { CalendarService.Scope.CalendarReadonly };
        public IActionResult Index()
        {
            UserCredential credential;

            using (var stream =
                new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
            }

            // Create Google Calendar API service.
            var service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "caledemo",
            });

            // Define parameters of request.
            EventsResource.ListRequest request = service.Events.List("primary");
            request.TimeMin = DateTime.Now;
            request.ShowDeleted = false;
            request.SingleEvents = true;
            request.MaxResults = 10;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

            // List events.
            Events events = request.Execute();
            // Google calendar data
            List<AppointmentData> appData = new List<AppointmentData>();
            if (events.Items != null && events.Items.Count > 0)
            {
                var i = 0;
                foreach (var eventItem in events.Items)
                {
                   
                    appData.Add(new AppointmentData
                    {
                        Id = i++,
                        Subject = eventItem.Summary,
                        StartTime = Convert.ToDateTime( eventItem.Start.DateTime),
                        EndTime = Convert.ToDateTime(eventItem.End.DateTime)
                    });
                }
            }
            //Schedule data
            List<AppointmentData> scheduleData = new List<AppointmentData>();
            scheduleData.Add(new AppointmentData
            {
                Id = 100,
                Subject = "Paris",
                StartTime = new DateTime(2020, 07, 23, 10, 0, 0),
                EndTime = new DateTime(2020, 07, 23, 12, 30, 0),
            });
            // Merge both schedule and google calendar data and assign it to the datasource of schedule
            List<AppointmentData> resultData = new List<AppointmentData>();
            resultData = appData.Concat(scheduleData).ToList();
            ViewBag.appointments = resultData;

            return View();
        }

        public IActionResult About()
        {
            string[] scopess = new string[] { CalendarService.Scope.Calendar };
            UserCredential credential;

            using (var stream =
                new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    scopess,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
            }

            // Create Google Calendar API service.
            var service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "caledemo",
            });
            var ev = new Event();
            EventDateTime start = new EventDateTime();
            start.DateTime = new DateTime(2020, 7, 21, 10, 0, 0);

            EventDateTime end = new EventDateTime();
            end.DateTime = new DateTime(2020, 7, 21, 10, 30, 0);


            ev.Start = start;
            ev.End = end;
            ev.Summary = "New Event";
            ev.Description = "Description...";

            var calendarId = "primary";
           
            Event recurringEvent = service.Events.Insert(ev, calendarId).Execute();
            ViewData["Message"] = "Event Created.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }
        
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet("Save")]
        public string Get()
        {
            return "value";
        }

    }
    public class AppointmentData
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
      
    }
}
