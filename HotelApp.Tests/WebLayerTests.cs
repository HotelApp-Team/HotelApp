using HotelApp.Web.Models;
using HotelApp.Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Text;
using System.Text.Json;
using WebAuthController = HotelApp.Web.Controllers.AuthController;

namespace HotelApp.Tests;

public class WebLayerTests
{
    [Fact]
    public async Task ApiService_GetAsync_SessionTokenVarsa_BearerHeaderEkler()
    {
        var handler = new CapturingHttpMessageHandler(_ => JsonResponse(HttpStatusCode.OK, new { id = 1 }));
        var session = new TestSession();
        session.SetString("JWToken", "session-token");
        var service = CreateApiService(handler, session);

        var result = await service.GetAsync<JsonElement>("hotels");

        Assert.True(result.Success);
        Assert.NotNull(handler.LastRequest);
        Assert.Equal("Bearer", handler.LastRequest!.Headers.Authorization?.Scheme);
        Assert.Equal("session-token", handler.LastRequest.Headers.Authorization?.Parameter);
    }

    [Fact]
    public async Task ApiService_GetAsync_HataCevabindaApiMessageOkur()
    {
        var handler = new CapturingHttpMessageHandler(_ => JsonResponse(
            HttpStatusCode.BadRequest,
            new { success = false, message = "API hata mesaji" }));
        var service = CreateApiService(handler, new TestSession());

        var result = await service.GetAsync<object>("hotels?page=0");

        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Equal("API hata mesaji", result.Message);
    }

    [Fact]
    public async Task WebAuth_LoginPost_BasariliysaSessionaTokenYazarVeHomeYonlendirir()
    {
        var response = new LoginResponse
        {
            Token = "jwt-token",
            Email = "user@hotelapp.test",
            FullName = "Test User",
            ExpiresAt = DateTime.UtcNow.AddMinutes(60)
        };
        var controller = CreateAuthController(JsonResponse(HttpStatusCode.OK, response), out var session);

        var result = await controller.Login(new LoginViewModel
        {
            Email = "user@hotelapp.test",
            Password = "Password123!"
        });

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        Assert.Equal("Home", redirect.ControllerName);
        Assert.Equal("jwt-token", session.GetString("JWToken"));
        Assert.Equal("user@hotelapp.test", session.GetString("UserEmail"));
        Assert.Equal("Test User", session.GetString("UserFullName"));
    }

    [Fact]
    public async Task WebAuth_LoginPost_ApiHatasiVarsaViewVeModelStateHatasiDoner()
    {
        var controller = CreateAuthController(JsonResponse(
            HttpStatusCode.Unauthorized,
            new { success = false, message = "Geçersiz email veya parola." }), out _);

        var result = await controller.Login(new LoginViewModel
        {
            Email = "user@hotelapp.test",
            Password = "wrong-password"
        });

        Assert.IsType<ViewResult>(result);
        Assert.False(controller.ModelState.IsValid);
        Assert.Contains(controller.ModelState[string.Empty]!.Errors,
            error => error.ErrorMessage == "Geçersiz email veya parola.");
    }

    [Fact]
    public async Task WebAuth_RegisterPost_Faz1TokenDonerseSessionaYazarVeHomeYonlendirir()
    {
        var response = new RegisterResponse
        {
            RequiresEmailVerification = false,
            Message = "Kayıt başarılı.",
            Token = "register-token",
            Email = "new-user@hotelapp.test",
            FullName = "New User",
            ExpiresAt = DateTime.UtcNow.AddMinutes(60)
        };
        var controller = CreateAuthController(JsonResponse(HttpStatusCode.OK, response), out var session);

        var result = await controller.Register(new RegisterViewModel
        {
            FirstName = "New",
            LastName = "User",
            Email = "new-user@hotelapp.test",
            Password = "Password123!",
            ConfirmPassword = "Password123!",
            Phone = "5551234567"
        });

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        Assert.Equal("Home", redirect.ControllerName);
        Assert.Equal("register-token", session.GetString("JWToken"));
        Assert.Equal("new-user@hotelapp.test", session.GetString("UserEmail"));
        Assert.Equal("New User", session.GetString("UserFullName"));
    }

    [Fact]
    public async Task WebAuth_VerifyEmail_TokenYoksaHataViewModeliDoner()
    {
        var controller = CreateAuthController(JsonResponse(HttpStatusCode.OK, new { }), out _);

        var result = await controller.VerifyEmail(token: null);

        var view = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<VerifyEmailViewModel>(view.Model);
        Assert.False(model.IsSuccess);
        Assert.Contains("eksik", model.Message);
    }

    [Fact]
    public async Task WebAuth_VerifyEmail_BasariliysaSessionaTokenYazar()
    {
        var response = new LoginResponse
        {
            Token = "verify-token",
            Email = "verified@hotelapp.test",
            FullName = "Verified User",
            ExpiresAt = DateTime.UtcNow.AddMinutes(60)
        };
        var controller = CreateAuthController(JsonResponse(HttpStatusCode.OK, response), out var session);

        var result = await controller.VerifyEmail("email-token");

        var view = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<VerifyEmailViewModel>(view.Model);
        Assert.True(model.IsSuccess);
        Assert.Equal("verified@hotelapp.test", model.Email);
        Assert.Equal("verify-token", session.GetString("JWToken"));
        Assert.Equal("verified@hotelapp.test", session.GetString("UserEmail"));
        Assert.Equal("Verified User", session.GetString("UserFullName"));
    }

    [Fact]
    public void WebAuth_Logout_SessioniTemizlerVeHomeYonlendirir()
    {
        var controller = CreateAuthController(JsonResponse(HttpStatusCode.OK, new { }), out var session);
        session.SetString("JWToken", "token");
        session.SetString("UserEmail", "user@hotelapp.test");
        session.SetString("UserFullName", "Test User");

        var result = controller.Logout();

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        Assert.Equal("Home", redirect.ControllerName);
        Assert.Null(session.GetString("JWToken"));
        Assert.Null(session.GetString("UserEmail"));
        Assert.Null(session.GetString("UserFullName"));
    }

    private static ApiService CreateApiService(CapturingHttpMessageHandler handler, TestSession session)
    {
        var httpContext = new DefaultHttpContext
        {
            Session = session
        };
        var accessor = new HttpContextAccessor
        {
            HttpContext = httpContext
        };
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://api.hotelapp.test/")
        };

        return new ApiService(httpClient, accessor);
    }

    private static WebAuthController CreateAuthController(HttpResponseMessage apiResponse, out TestSession session)
    {
        var handler = new CapturingHttpMessageHandler(_ => apiResponse);
        session = new TestSession();
        var httpContext = new DefaultHttpContext
        {
            Session = session
        };
        var accessor = new HttpContextAccessor
        {
            HttpContext = httpContext
        };
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://api.hotelapp.test/")
        };
        var apiService = new ApiService(httpClient, accessor);

        // S5-02 sonrasi AuthController ctor IConfiguration alir (IsAuthenticated icin Jwt:* gerekli).
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:SecretKey"] = "BuCokGizliBirAnahtarOlmaliEnAz32Karakter12345",
                ["Jwt:Issuer"] = "HotelApp.API",
                ["Jwt:Audience"] = "HotelApp.Client"
            })
            .Build();

        return new WebAuthController(apiService, configuration)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            }
        };
    }

    private static HttpResponseMessage JsonResponse(HttpStatusCode statusCode, object body)
    {
        return new HttpResponseMessage(statusCode)
        {
            Content = new StringContent(
                JsonSerializer.Serialize(body),
                Encoding.UTF8,
                "application/json")
        };
    }

    private sealed class CapturingHttpMessageHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _responseFactory;

        public CapturingHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> responseFactory)
        {
            _responseFactory = responseFactory;
        }

        public HttpRequestMessage? LastRequest { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LastRequest = request;
            return Task.FromResult(_responseFactory(request));
        }
    }
}
