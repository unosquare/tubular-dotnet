﻿namespace Unosquare.Tubular.EmbedioSample
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Labs.EmbedIO;
    using Labs.EmbedIO.Modules;
    using ObjectModel;
    using Swan;
#if NETCOREAPP2_0
    using System.Net;
#else
    using Net;
#endif

    /// <summary>
    /// A simple model representing a person
    /// </summary>
    public class Person
    {
        public int Key { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string EmailAddress { get; set; }
        public string PhotoUrl { get; set; }
    }

    /// <summary>
    /// A very simple controller to handle People CRUD.
    /// Notice how it Inherits from WebApiController and the methods have WebApiHandler attributes 
    /// This is for sampling purposes only.
    /// </summary>
    public class PeopleController : WebApiController
    {
        private const string RelativePath = "/api/";

        public static List<Person> People = new List<Person>
        {
            new Person {Key = 1, Name = "Mario Di Vece", Age = 31, EmailAddress = "mario@unosquare.com"},
            new Person {Key = 2, Name = "Geovanni Perez", Age = 31, EmailAddress = "geovanni.perez@unosquare.com"},
            new Person {Key = 3, Name = "Luis Gonzalez", Age = 29, EmailAddress = "luis.gonzalez@unosquare.com"},
            new Person {Key = 4, Name = "Ricardo Salinas", Age = 22, EmailAddress = "ricardo.salinas@unosquare.com"}
        };

        [WebApiHandler(Labs.EmbedIO.Constants.HttpVerbs.Post, RelativePath + "people")]
        public bool GetPeople(WebServer server, HttpListenerContext context)
        {
            try
            {
                var model = context.ParseJson<GridDataRequest>();

                return context.JsonResponse(model.CreateGridDataResponse(People.AsQueryable()));
            }
            catch (Exception ex)
            {
                // here the error handler will respond with a generic 500 HTTP code a JSON-encoded object
                // with error info. You will need to handle HTTP status codes correctly depending on the situation.
                // For example, for keys that are not found, ou will need to respond with a 404 status code.
                return HandleError(context, ex);
            }
        }

        /// <summary>
        /// Handles the error returning an error status code and json-encoded body.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="ex">The ex.</param>
        /// <param name="statusCode">The HTTP status code.</param>
        /// <returns></returns>
        protected bool HandleError(HttpListenerContext context, Exception ex, int statusCode = 500)
        {
            var errorResponse = new
            {
                Title = "Unexpected Error",
                ErrorCode = ex.GetType().Name,
                Description = ex.ExceptionMessage(),
            };

            context.Response.StatusCode = statusCode;
            return context.JsonResponse(errorResponse);
        }
    }
}
