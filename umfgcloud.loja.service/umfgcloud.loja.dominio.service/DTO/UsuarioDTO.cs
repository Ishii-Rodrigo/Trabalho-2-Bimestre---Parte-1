using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace umfgcloud.loja.dominio.service.DTO
{
    public sealed class UsuarioDTO
    {
        public abstract class AbstractUsuario
        {
            [JsonPropertyName("email")]
            [Required(ErrorMessage = "O atributo email é obrigatório.")]
            [EmailAddress(ErrorMessage = "O valor do atributo email é inválido.")]
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public string Email { get; set; } = string.Empty;

            [JsonPropertyName("password")]
            [Required(ErrorMessage = "O atributo password é obrigatório")]
            [StringLength(50, 
                ErrorMessage = "O atributo passward deve ter entre 6 e 50 caracteres.",
                MinimumLength = 6)]
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public string Password { get; set; } = string.Empty;

        }

        public sealed class SingInRequest : AbstractUsuario { }

        public sealed class SingInResponse
        {
            [JsonPropertyName("acessToken")]
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public string AcessToken { get; set; } = string.Empty;
        }

        public sealed class SingUpRequest : AbstractUsuario
        {
            [JsonPropertyName("confirmedPassword")]
            [Compare(nameof(Password), ErrorMessage = "As senhas devem ser iguais.")]
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public string ConfirmedPassword { get; set; } = string.Empty;
        }
    }
}