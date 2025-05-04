using Microsoft.AspNetCore.Mvc;
using SistemaLeilao_web.Models;
using SistemaLeilao_web.Settings;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Net.Http.Headers; // Required for Authorization header
using System.Net.Http.Json;
using System.Text.Json;
using System.Text;

namespace SistemaLeilao_web.Controllers
{
    public class LeilaoController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ApiSettings _apiSettings;

        // Inject HttpClientFactory or configure HttpClient properly for production
        public LeilaoController(IOptions<ApiSettings> apiSettings)
        {
            _apiSettings = apiSettings.Value;
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(_apiSettings.BaseUrl)
            };
        }

        // Helper method to set Authorization header
        private void SetAuthorizationHeader()
        {
            var token = HttpContext.Request.Cookies["AuthToken"];
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                 _httpClient.DefaultRequestHeaders.Authorization = null; // Clear if no token
            }
        }

        // GET: Leilao/Cadastro
        public IActionResult Cadastro()
        {
            // Return the view for creating a new auction
            return View();
        }

        // POST: Leilao/SalvarCadastro (AJAX)
        [HttpPost]
        // [ValidateAntiForgeryToken] // Consider adding if needed for AJAX POST
        public async Task<IActionResult> SalvarCadastro([FromBody] LeilaoModel model)
        {
            SetAuthorizationHeader(); // Set token before calling API
            if (string.IsNullOrEmpty(HttpContext.Request.Cookies["AuthToken"]))
            {
                 return Json(new { success = false, message = "Usuário não autenticado." });
            }

            try
            {
                var apiUrl = _apiSettings.BaseUrl.EndsWith("/") ? "api/leiloes" : "/api/leiloes";
                var response = await _httpClient.PostAsJsonAsync(apiUrl, model);

                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Leilão cadastrado com sucesso!", redirectUrl = Url.Action("Index", "Home") });
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"API Leilao Cadastro Error: {response.StatusCode} - {errorContent}");
                    string errorMessage = "Erro ao cadastrar o leilão.";
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
                Console.WriteLine($"Erro interno no SalvarCadastro Leilao: {ex.Message}");
                return Json(new { success = false, message = "Ocorreu um erro interno ao tentar salvar o leilão." });
            }
        }

        // GET: Leilao/GetLeiloesAtivos (AJAX)
        [HttpGet]
        public async Task<IActionResult> GetLeiloesAtivos()
        {
             SetAuthorizationHeader(); // Set token before calling API
             if (string.IsNullOrEmpty(HttpContext.Request.Cookies["AuthToken"]))
            {
                 // Decide if unauthorized users can see auctions or return error
                 // For now, let's assume they can't and return an error/empty list
                 // return Json(new { success = false, message = "Usuário não autenticado." }); 
                 // OR return empty list:
                 return Json(new List<LeilaoModel>()); 
            }

            try
            {
                var apiUrl = _apiSettings.BaseUrl.EndsWith("/") ? "api/leiloes/ativos" : "/api/leiloes/ativos"; // Assuming this endpoint exists
                var response = await _httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var leiloes = await response.Content.ReadFromJsonAsync<List<LeilaoModel>>();
                    return Json(leiloes ?? new List<LeilaoModel>()); // Return auctions or empty list
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"API GetLeiloesAtivos Error: {response.StatusCode} - {errorContent}");
                    // Return empty list or error object
                    return Json(new List<LeilaoModel>()); // Return empty list on error for simplicity
                    // OR return Json(new { success = false, message = "Erro ao buscar leilões." });
                }
            }
            catch (Exception ex)
            {
                 Console.WriteLine($"Erro interno no GetLeiloesAtivos: {ex.Message}");
                 return Json(new List<LeilaoModel>()); 
            }
        }


        // Placeholder for other auction-related actions (My Bids, History, Search)
        public IActionResult MeusLances()
        {
            // TODO: Implement view to show user's bids
            ViewData["Title"] = "Meus Lances";
            // Pass necessary data to the view
            return View();
        }

        public IActionResult HistoricoLeiloes()
        {
            // TODO: Implement view to show user's created auctions
            ViewData["Title"] = "Histórico de Leilões";
            // Pass necessary data to the view
            return View();
        }

         public IActionResult BuscarLances()
        {
            // TODO: Implement view to search for auctions/bids
            ViewData["Title"] = "Buscar Lances";
            // Pass necessary data to the view
            return View();
        }
    }
}

