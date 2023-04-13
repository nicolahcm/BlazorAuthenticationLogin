**Important Steps

1. In App.razor, add the followings:
    -CascadingAuthenticationState, AuthorizeRouteView, NotAuthorized
    -And Remove RouteView. 
    The result is like the following.
```
<CascadingAuthenticationState>
    <Router AppAssembly="@typeof(App).Assembly">
        <Found Context="routeData">
            <AuthorizeRouteView RouteData="@routeData" DefaultLayout="@typeof(MainLayout)">
                <NotAuthorized>
                    You're not allowed to see this page.
                </NotAuthorized>
            </AuthorizeRouteView>
            <FocusOnNavigate RouteData="@routeData" Selector="h1" />
        </Found>
        <NotFound>
            <PageTitle>Not found</PageTitle>
            <LayoutView Layout="@typeof(MainLayout)">
                <p role="alert">Sorry, there's nothing at this address.</p>
            </LayoutView>
        </NotFound>
    </Router>
</CascadingAuthenticationState>
```

2. Add the class CustomAuthStateProvider.cs as in this snippet code.
    -It has to implement the interface AuthenticationStateProvider, in particular its method GetAuthenticationStateAsync
    -Use LocalStorage library for retrieving values stored in the local storage and parsing claims from Jwt

3. To Cover a Page there are 2 ways:
    -At the beginning of the razor page, add the following: 
    ```
    @attribute [Authorize]  // from Microsoft.AspNetCore.Authorization.Authorize
    ```
    -Or inside a razor page use the following code:
    ```
    <AuthorizeView>
        <NotAuthorized>
            You're not authorized
        </NotAuthorized>
        <Authorized>
            Ok, you're authorized as @context.User.Claims.Where(c => c.Type == "name").FirstOrDefault().Value.ToString()
        </Authorized>
    </AuthorizeView>
    ```
4. Add a Controller for login, taking as input username and password. If correct, returns a token string
    (remember to use app.MapControllers() in Program.cs)

5. Create a Login page:
    -Login.razor:
        when clicking login it should take care of the following actions:
            1) Send http post to the login controller route
            2) save the token (if successful) in localstorage
            3) Call the method AuthenticationStateProvider.GetAuthenticationStateAsync() // for updating the views
            4) Redirect to the home page

Dependencies: 
```
    <PackageReference Include="Blazored.LocalStorage" Version="4.3.0" />
    <PackageReference Include="Microsoft.AspNetCore.Components.Authorization" Version="6.0.16" />
```

6. In Program.cs add the following:
```
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddBlazoredLocalStorage(); // For injecting IlocalStorage
builder.Services.AddHttpClient();
builder.Services.AddAuthorizationCore();

app.MapControllers();
```

7. Add the LoginLogoutButton.razor --> The Button for login/logout

------------------
Functional Explanation
-----------------

The most imporant things are 2: 
-CustomAuthStateProvider:
    1) When  var identity = new ClaimsIdentity(); Then it is not authorized. 
    When instead we have some arguments identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt"), then it is authorized.

    2) NotifyAuthenticationStateChanged(Task.FromResult(state)); Tells all the UI components from points 1. and 3. Above, that
    there has been an update, so they will update their rendering.
-Login.razor for the reasons explained above.



----------------------
MORE 1
----------------------
In Production use a JwtManagerTokenGenerator, see the project https://github.com/nicolahcm/JWT-Cryptography-DotNet.
In partciular, the auth route controller, should check that the user belongs to a list in db, and then generates the token.
Moreover, the method AuthenticationStateProvider.GetAuthenticationStateAsync() should also validate the token,
given the symmetric key used to generate the token in the auth route controller (AuthController in our case). 
With the project linked, you can also set an expiry date.

------------------
MORE 2 
------------------
The AuthController, should take the user and password from a db. Password hashed in the db


-----------------
MORE 3
-----------------
Once the user has successfully logged in, all the Http requests from the UI should include a header Authorization
"Bearer ..JWT.." that they send to other services. There exists a method, such as http.SetFixedHeaders(..)


