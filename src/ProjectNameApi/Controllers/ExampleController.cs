using Microsoft.AspNetCore.Mvc;
using ProjectNameApi.Services;

namespace ProjectNameApi.Controllers
{
    [Route("example")]
    public class ExampleController : ApiController
    {
        private readonly ExampleService _exampleService;
        
        public ExampleController(ExampleService exampleService)
        {
            _exampleService = exampleService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(OkObjectResult), 200)]
        public IActionResult GetExampleConfigValue()
        {
            return Ok(_exampleService.ExampleMethod());
        }
    }
}