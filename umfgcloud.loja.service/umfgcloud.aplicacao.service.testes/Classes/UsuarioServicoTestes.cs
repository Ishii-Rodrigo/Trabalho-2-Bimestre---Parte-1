using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace umfgcloud.aplicacao.service.testes.Classes;

[TestClass]
public class UsuarioServicoTestes : AbstractServicoTestes
{

    private const string C_OWNER = "Juliano Maciel";
    private const string C_CATEGORY = "cadastro - login";

    [TestMethod]
    [Owner(C_OWNER)]
    [TestCategory(C_CATEGORY)]
    public async Task UsuarioServico_CadastrarAsync_Sucesso()
    {
        try
        {
            var userManager = GetUserManagerSuccess();
            var roleManager = GetRoleManagerExistsTrue();
            var siginManager = GetSignInManagerSuccess();
            var usuarioServico = GetUsuarioServico(userManager,
                roleManager, siginManager);
            var dto = GetSingUpRequestDTO();

            await usuarioServico.CadastrarAsync(dto);
        }
        catch (Exception ex) 
        { 
            Assert.Fail(ex.Message);
        }
    }

    [TestMethod]
    [Owner(C_OWNER)]
    [TestCategory(C_CATEGORY)]
    public async Task UsuarioServico_CadastrarAsync_Falha()
    {
        try
        {
            var userManager = GetUserManagerFailed();
            var roleManager = GetRoleManagerExistsTrue();
            var siginManager = GetSignInManagerSuccess();
            var usuarioServico = GetUsuarioServico(userManager,
                roleManager, siginManager);
            var dto = GetSingUpRequestDTO();

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => usuarioServico.CadastrarAsync(dto));
        }
        catch (Exception ex)
        {
            Assert.Fail(ex.Message);
        }
    }

    [TestMethod]
    [Owner(C_OWNER)]
    [TestCategory(C_CATEGORY)]
    public async Task UsuarioServico_CadastrarAsync_FalhaUsuarioNaoEncontrado()
    {
        try
        {
            var userManager = GetUserManagerSuccess();
            var roleManager = GetRoleManagerWithoutFindByNameAsync();
            var siginManager = GetSignInManagerSuccess();
            var usuarioServico = GetUsuarioServico(userManager,
                roleManager, siginManager);
            var dto = GetSingUpRequestDTO();

            await Assert.ThrowsExceptionAsync<ArgumentException>(
                () => usuarioServico.CadastrarAsync(dto));
        }
        catch (Exception ex)
        {
            Assert.Fail(ex.Message);
        }
    }
}