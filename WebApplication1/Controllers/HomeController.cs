using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using kp.Infrastructure.KeyCloakClient;
using kp.Infrastructure.KeyCloakClient.Logout;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using WebAppDemo.Models;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
	public class HomeController : Controller
	{
		private readonly IConfiguration Configuration;

		public HomeController(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IActionResult Index()
		{
			return View();
		}

		[Authorize]
		public IActionResult Privacy()
		{
			string token = HttpContext.GetTokenAsync("access_token").Result;
			string idToken = HttpContext.GetTokenAsync("id_token").Result;
			string refreshToken = HttpContext.GetTokenAsync("refresh_token").Result;

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

		public async Task<IActionResult> LogOff()
		{
			Authority authority = Configuration.GetSection(nameof(Authority)).Get<Authority>();
			string accessToken = HttpContext.GetTokenAsync("access_token").Result;
			string refreshToken = HttpContext.GetTokenAsync("refresh_token").Result;

			LogoutRequest logoutRequest = new LogoutRequest(authority.EndpointAddress, authority.RealmName, authority.Protocol,
															accessToken, refreshToken, authority.ClientId, authority.ClientSecret);

			KeyCloakClientUtility adapter = new KeyCloakClientUtility();
			await adapter.Logout(logoutRequest);

			Request.Cookies.Keys.ToList().ForEach(x => Response.Cookies.Delete(x));

			return RedirectToAction("Index");
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

		public async Task<IActionResult> ApiCallAdmin()
		{
			var token = HttpContext.GetTokenAsync("access_token");

			var client = new HttpClient();
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Result);
			var content = await client.GetStringAsync("https://localhost:44373/getAdmin");


			List<WeatherForecastModel> model = Newtonsoft.Json.JsonConvert.DeserializeObject<List<WeatherForecastModel>>(content);
			ViewData.Model = model;
			ViewBag.ApiUser = "Admin";

			return View("ApiCall");
		}

		public async Task<IActionResult> ApiCallAnonymous()
		{

			var client = new HttpClient();
			var content = await client.GetStringAsync("https://localhost:44373/getfree");


			List<WeatherForecastModel> model = Newtonsoft.Json.JsonConvert.DeserializeObject<List<WeatherForecastModel>>(content);
			ViewData.Model = model;
			ViewBag.ApiUser = "Anonymous";

			return View("ApiCall");
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
