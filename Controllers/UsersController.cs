using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Roomie.WebAPI.Model;

namespace Roomie.WebAPI.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("api/[controller]/")]
    public class UsersController : ControllerBase
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<UsersController> _logger;
        public UsersController(ILogger<UsersController> logger, IHostingEnvironment hostingEnvironment)
        {
            _logger = logger;
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpGet]
        [Route("allUser")]
        public async Task<IActionResult> Get()
        {
            string contentRootPath = _hostingEnvironment.ContentRootPath;
            var JSON = await System.IO.File.ReadAllTextAsync(contentRootPath + "/data.json");
            var userList = UserModel.FromJson(JSON);
            return Ok(userList);
        }
        [HttpPost]
        [AllowAnonymous]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            string contentRootPath = _hostingEnvironment.ContentRootPath;
            var JSON = await System.IO.File.ReadAllTextAsync(contentRootPath + "/data.json");
            var userList = UserModel.FromJson(JSON);
            var user = userList.Where(x => x.Email.ToLower() == model.username.ToLower() && x.Password.ToLower() == model.username.ToLower());
            string token = "";
            if (user != null)
            {
                token = Guid.NewGuid().ToString();
            }
            return Ok(new { authToken = token });
        }
        [HttpPost]
        [AllowAnonymous]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] UserModel model)
        {
            string contentRootPath = _hostingEnvironment.ContentRootPath;
            var JSON = await System.IO.File.ReadAllTextAsync(contentRootPath + "/data.json");
            var userList = UserModel.FromJson(JSON);
            model.Id = userList.Count+1;
            userList.Add(model);
            JSON = Serialize.ToJson(userList);
            int state = -1;
            string message = "";
            try
            {
                await System.IO.File.WriteAllTextAsync(contentRootPath + "/data.json", JSON);
                state = 1;
                message = "Success";
            }
            catch (System.Exception ex)
            {
                message = ex.Message;

            }
            return Ok(new { state = state, message = message });
        }
    }
}
