using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Text.Json;





namespace BlazorAppAuthenticationLogin
{
	public class CustomAuthStateProvider : AuthenticationStateProvider
	{
		private readonly ILocalStorageService _localStorage;

		public CustomAuthStateProvider(ILocalStorageService localStorage)
		{
			_localStorage = localStorage;
		}

		public override async Task<AuthenticationState> GetAuthenticationStateAsync()
		{

			// Not Authorized
			var identity = new ClaimsIdentity();

			string token = null;
			try
			{
				token = await _localStorage.GetItemAsStringAsync("token");
			}
			catch (Exception ex)
			{
				// Console.WriteLine(ex.ToString()); 
				Console.WriteLine("Error in retrieving token");
			}

			if (!string.IsNullOrEmpty(token))
			{
				// TO DO: Check JWT Is VALID
				// ------------------

				try
				{
					// Authorized 
					identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");
				}
				catch (Exception ex)
				{
					// Console.WriteLine(ex.ToString());
					Console.WriteLine("Error in parsing token"); // Or other type of errors? 
				}
			}

			var user = new ClaimsPrincipal(identity);
			var state = new AuthenticationState(user);
			// SET the Auth State Again
			NotifyAuthenticationStateChanged(Task.FromResult(state));

			return state;
			// return Task.FromResult(state);
		}


		public static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
		{
			var payload = jwt.Split('.')[1];
			var jsonBytes = ParseBase64WithoutPadding(payload);
			var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
			return keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()));
		}

		private static byte[] ParseBase64WithoutPadding(string base64)
		{
			switch (base64.Length % 4)
			{
				case 2: base64 += "=="; break;
				case 3: base64 += "="; break;
			}
			return Convert.FromBase64String(base64);
		}
	}
}
