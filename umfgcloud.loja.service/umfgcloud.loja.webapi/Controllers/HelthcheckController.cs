using Microsoft.AspNetCore.Mvc;

namespace umfgcloud.loja.webapi.Controllers
{
    /// <summary>
    /// endpoint(s) de verificação de saúde da API
    /// </summary>
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "v1")]
    [ApiController]
    [Route("[controller]")] //toda classe controller deve possuir o sufixo controller 
    public sealed class HelthcheckController : ControllerBase
    {
        /// <summary>
        /// verifica o estado de saúde da API
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Ping()
        {
            return Ok($"umfgcloud.loja.service -> online | {DateTime.Now}");
        }

        //exemplo como usando o Route de atributo na classe assume automaticamente
        //a rota na uri implementando o protocolo http
        //[HttpPost]
        //public IActionResult Post()
        //{
        //    return Ok("Teste Post");
        //}
    }
}