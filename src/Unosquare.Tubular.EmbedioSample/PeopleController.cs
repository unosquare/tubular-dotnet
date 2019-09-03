using System.Collections.Generic;
using System.Linq;
using EmbedIO;
using EmbedIO.Routing;
using EmbedIO.WebApi;
using Unosquare.Tubular.ObjectModel;

namespace Unosquare.Tubular.EmbedioSample
{
    /// <summary>
    /// A very simple controller to handle People CRUD.
    /// </summary>
    public class PeopleController : WebApiController
    {
        private static readonly List<Person> People = new List<Person>
        {
            new Person {Key = 1, Name = "Mario Di Vece", Age = 31, EmailAddress = "mario@unosquare.com"},
            new Person {Key = 2, Name = "Geovanni Perez", Age = 31, EmailAddress = "geovanni.perez@unosquare.com"},
            new Person {Key = 3, Name = "Luis Gonzalez", Age = 29, EmailAddress = "luis.gonzalez@unosquare.com"},
            new Person {Key = 4, Name = "Carlos Solorzano", Age = 22, EmailAddress = "carlos.solorzano@unosquare.com"}
        };

        [Route(HttpVerbs.Post, "/people")]
        public GridDataResponse GetPeople([JsonGridDataRequest] GridDataRequest model) => model.CreateGridDataResponse(People.AsQueryable());
    }
}
