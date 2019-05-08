namespace Unosquare.Tubular.EmbedioSample
{
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using System.Linq;
    using Labs.EmbedIO;
    using Labs.EmbedIO.Modules;
    using ObjectModel;

    /// <summary>
    /// A simple model representing a person
    /// </summary>
    public class Person
    {
        public int Key { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string EmailAddress { get; set; }
    }

    /// <summary>
    /// A very simple controller to handle People CRUD.
    /// Notice how it Inherits from WebApiController and the methods have WebApiHandler attributes 
    /// This is for sampling purposes only.
    /// </summary>
    public class PeopleController : WebApiController
    {
        private const string RelativePath = "/api/";

        private static readonly List<Person> People = new List<Person>
        {
            new Person {Key = 1, Name = "Mario Di Vece", Age = 31, EmailAddress = "mario@unosquare.com"},
            new Person {Key = 2, Name = "Geovanni Perez", Age = 31, EmailAddress = "geovanni.perez@unosquare.com"},
            new Person {Key = 3, Name = "Luis Gonzalez", Age = 29, EmailAddress = "luis.gonzalez@unosquare.com"},
            new Person {Key = 4, Name = "Ricardo Salinas", Age = 22, EmailAddress = "ricardo.salinas@unosquare.com"}
        };


        public PeopleController(IHttpContext context) : base(context)
        {
        }

        [WebApiHandler(Labs.EmbedIO.Constants.HttpVerbs.Post, RelativePath + "people")]
        public async Task<bool> GetPeople()
        {
            var model = await HttpContext.ParseJsonAsync<GridDataRequest>();

            return await Ok(model.CreateGridDataResponse(People.AsQueryable()));
        }
    }
}
