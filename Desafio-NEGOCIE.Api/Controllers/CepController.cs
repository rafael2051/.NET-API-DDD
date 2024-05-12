using Microsoft.AspNetCore.Mvc;
using DesafioNEGOCIE.Application.Services.RegistrationCep;
using DesafioNEGOCIE.Application.Services.RequisitionCep;
using Newtonsoft.Json;
using System.Data;
using Npgsql;

namespace DesafioNEGOCIE.Api.Controllers;

[ApiController]
[Route("api")]
public class CepController : ControllerBase
{

    private readonly IRequisitionCepService _requisitionCepService;
    private readonly IRegistrationCepService _registrationCepService;

    //Injeção de dependência
    public CepController(IRequisitionCepService requisitionCepService, IRegistrationCepService registerCepService)
    {
        _requisitionCepService = requisitionCepService;
        _registrationCepService = registerCepService;
    }

    [HttpPost("cep/{cep}")]
    public async Task<IActionResult> PostCEPController(string cep)
    {

        RegistrationCepResponse response;

        Console.WriteLine("-------------------");
        Console.WriteLine($"Recebida soliticação de post para o cep {cep}\n");

        try
        {
            response = await _registrationCepService.RegisterCep(cep);
        }
        catch (ArgumentException ex)
        {
            //Indicamos que a string do cep passada não corresponse ao padrão de um cep
            return StatusCode(400, ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            //Indicamos que o cep não existe
            return StatusCode(404, ex.Message);
        }
        catch (DuplicateNameException ex)
        {
            //Tentativa de duplicar registro no banco de dados
            return StatusCode(409, ex.Message);
        }
        catch (HttpRequestException ex)
        {
            //Caso a API dos correios esteja inacessível
            return StatusCode(502, ex.Message);
        }
        catch (NpgsqlException ex)
        {
            //Algum problema ao acessar o banco de dados
            return StatusCode(500, "Erro ao acessar o banco de dados!\n"
                                    + $"\t{ex.Message}");
        }
        catch (Exception ex)
        {
            //Algum outro problema interno mais genérico
            return StatusCode(500, "Erro interno no servidor!\n"
                                    + $"\t{ex.Message}");
        }

        return StatusCode(response.httpCode, response.mensagem);
    }

    [HttpGet("cep/{cep}")]
    public IActionResult GetCEPController(string cep)
    {

        RequisitionCepResponse response;

        Console.WriteLine("-------------------");
        Console.WriteLine($"Recebida solicitação de get para o cep {cep}\n");

        try
        {
            response = _requisitionCepService.RequestCep(cep);
        }
        catch (ArgumentException ex)
        {
            return StatusCode(400, ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return StatusCode(404, ex.Message);
        }
        catch (NpgsqlException ex)
        {
            return StatusCode(500, "Erro ao acessar o banco de dados\n"
                                    + $"\t{ex.Message}");
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Erro interno no servidor!\n"
                                    + $"\t{ex.Message}");
        }


        // Configurações de serialização
        var settings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented // Formatação para facilitar a visualização
        };

        //Serialização do objeto retornado pela camada de aplicação
        var json = JsonConvert.SerializeObject(response.endereco, settings);

        Console.WriteLine("Enviando resposta ao cliente: \n"
                            + json);

        return StatusCode(200, json);
    }

}