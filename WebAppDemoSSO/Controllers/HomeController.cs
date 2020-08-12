using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebAppDemoSSO.Models;

namespace WebAppDemoSSO.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;

		public HomeController(ILogger<HomeController> logger)
		{
			_logger = logger;
		}

		public IActionResult Index()
		{
			return View();
		}

		[Authorize]
		public IActionResult Privacy()
		{
			var claims = User.Claims;

			PersonInfo person = new PersonInfo
			{
				Email = claims.FirstOrDefault(c => c.Type == "email")?.Value,
				FirstName = claims.FirstOrDefault(c => c.Type == "given_name")?.Value,
				LastName = claims.FirstOrDefault(c => c.Type == "family_name")?.Value,
				OIB = claims.FirstOrDefault(c => c.Type == "oib")?.Value
			};

			ViewData.Model = person;

			return View();
		}

		public async Task<IActionResult> ApiCall()
		{
			var token = HttpContext.GetTokenAsync("access_token");

			var client = new HttpClient();
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Result);
			var content = await client.GetStringAsync("https://localhost:44373/weatherforecast");


			List<WeatherForecastModel> model = Newtonsoft.Json.JsonConvert.DeserializeObject<List<WeatherForecastModel>>(content);
			ViewData.Model = model;
			ViewBag.ApiUser = "User";

			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
