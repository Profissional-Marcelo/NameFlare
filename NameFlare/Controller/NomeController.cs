using Microsoft.AspNetCore.Mvc;

namespace NameFlare.Controllers
{
    [ApiController]
    [Route("busca/[controller]")]
    public class NomeController : ControllerBase
    {
        [HttpGet("{nome}")]
        public IActionResult GetMaisPesquisados(string nome)
        {
            var resultados = new[]
            {
                $"{nome} Silva",
                $"{nome} Souza",
                $"{nome} Oliveira",
                $"{nome} Costa",
                $"{nome} Lima"
            };

            return Ok(resultados);
        }
    }
}
