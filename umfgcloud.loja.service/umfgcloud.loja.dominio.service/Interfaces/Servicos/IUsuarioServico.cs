using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using umfgcloud.loja.dominio.service.DTO;

namespace umfgcloud.loja.dominio.service.Interfaces.Servicos
{
    public interface IUsuarioServico
    {
        Task CadastrarAsync(UsuarioDTO.SingUpRequest dto);
        Task<UsuarioDTO.SingInResponse> AutenticarAsync(UsuarioDTO.SingInRequest dto);
    }
}
