using BlazorAppAuthenticationLogin.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlazorAppAuthenticationLogin.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		[Microsoft.AspNetCore.Mvc.HttpPost]
		public async Task<ActionResult<string>> Login(UserLoginDto request)
		{
			string token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c";

			string adminUsername = "user";
			string adminPassword = "password";

			if (request.Username.Equals(adminUsername) && request.Password.Equals(adminPassword))
			{
				return token;
			}

			// TO DO: Generate Token+
			// -------
			// TO DO LATER


			return "ERROR";
		}

	}
}
