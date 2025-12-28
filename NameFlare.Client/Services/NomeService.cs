using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace NameFlare.Client.Services
{
    public class NomeService
    {
        private readonly HttpClient _http;

        public NomeService(HttpClient http)
        {
            _http = http;
        }

        public async Task<string[]> BuscarMaisPesquisadosAsync(string nome)
        {
            return await _http.GetFromJsonAsync<string[]>($"busca/Nome/{nome}")
                   ?? Array.Empty<string>();
        }
    }
}
