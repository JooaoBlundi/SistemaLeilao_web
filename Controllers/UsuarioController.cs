using Microsoft.AspNetCore.Mvc;
using SistemaLeilao_web.Models;
using SistemaLeilao_web.Settings;
using System.Net.Http;
using System.Net.Http.Json; // Add this for PostAsJsonAsync
using System.Text.Json;
using System.Text;
using Microsoft.Extensions.Options;

namespace SistemaLeilao_web.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ApiSettings _apiSettings;

        public UsuarioController(IOptions<ApiSettings> apiSettings)
        {
            _apiSettings = apiSettings.Value;
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(_apiSettings.BaseUrl)
            };
        }

        public IActionResult Login()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginModel model) 
        {
            try
            {
                HttpResponseMessage response = await _httpClient.PostAsJsonAsync($"{_apiSettings.BaseUrl}/auth/login", model);

                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadFromJsonAsync<JsonElement>();
                    if (responseData.TryGetProperty("token", out var tokenElement))
                    {
                        var token = tokenElement.GetString();
                        var cookieOptions = new CookieOptions 
                        {
                            HttpOnly = true, 
                            Secure = true, 
                            SameSite = SameSiteMode.Strict 
                        };

                        if (model.ManterConectado) 
                        {
                            cookieOptions.Expires = DateTimeOffset.UtcNow.AddHours(1); 
                        }

                        HttpContext.Response.Cookies.Append("AuthToken", token, cookieOptions);
                        return Json(new { success = true, redirectUrl = Url.Action("Index", "Home") });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Resposta da API inválida (token não encontrado)." });
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"API Login Error: {response.StatusCode} - {errorContent}"); 
                    return Json(new { success = false, message = "Usuário ou senha inválido." }); // Generic message for security
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro interno no Login: {ex.Message}");
                return Json(new { success = false, message = "Ocorreu um erro interno ao tentar fazer login." });
            }
        }

        public IActionResult Cadastro()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> SalvarCadastro([FromBody] UsuarioModel usuario) 
        {
            try
            {
                HttpResponseMessage response = await _httpClient.PostAsJsonAsync($"{_apiSettings.BaseUrl}/usuario/add", usuario);

                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Cadastro realizado com sucesso!", redirectUrl = Url.Action("Login", "Usuario") });
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"API Cadastro Error: {response.StatusCode} - {errorContent}");
                    string errorMessage = "Erro ao realizar o cadastro. Verifique os dados e tente novamente.";
                    try {
                        var errorData = JsonSerializer.Deserialize<JsonElement>(errorContent);
                        if (errorData.TryGetProperty("errors", out var errorsElement) && errorsElement.ValueKind == JsonValueKind.Array) {
                            errorMessage = string.Join(" ", errorsElement.EnumerateArray().Select(e => e.GetString()));
                        } else if (errorData.TryGetProperty("message", out var messageElement)) {
                            errorMessage = messageElement.GetString() ?? errorMessage;
                        }
                    } catch {}

                    return Json(new { success = false, message = errorMessage });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro interno no SalvarCadastro: {ex.Message}");
                return Json(new { success = false, message = "Ocorreu um erro interno ao tentar salvar o cadastro." });
            }
        }
    }
}

