using Microsoft.AspNetCore.Mvc;

namespace umfgcloud.loja.webapi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public abstract class AbstractController : ControllerBase
    {
        protected async Task<IActionResult> InvokeMethodAsync<T, TResult>(
            Func<T, Task<TResult>> method, T args)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(GetMessageErrors());

                return Ok(await method.Invoke(args));
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        protected async Task<IActionResult> InvokeMethodAsync<TResult>(Func<Task<TResult>> method)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(GetMessageErrors());

                return Ok(await method.Invoke());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private string GetMessageErrors()
            => string.Join("\n", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
    }
}