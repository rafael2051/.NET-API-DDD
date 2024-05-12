using System.Data;
using System.Runtime.ConstrainedExecution;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using DesafioNEGOCIE.Domain.Entities;
using DesafioNEGOCIE.Infrastructure.Persistence;
using Microsoft.Data.SqlClient;

namespace DesafioNEGOCIE.Application.Services.RegistrationCep;

public class RegistrationCepService : IRegistrationCepService
{

    private readonly IEnderecoRepository _enderecoRepository;

    public RegistrationCepService(IEnderecoRepository enderecoRepository)
    {
        _enderecoRepository = enderecoRepository;
    }

    public async Task<RegistrationCepResponse> RegisterCep(string cep)
    {

        //Para consumir a api do correio
        string url = $"https://viacep.com.br/ws/{cep}/json/";

        //Objeto usado para fazer a persistência no banco de dados
        Endereco? endereco;

        // 1. Validar se o campo do cep veio vazio

        if (cep.Length == 0)
        {
            throw new ArgumentNullException("O cep passado é nulo!");
        }

        // 2. Validar de se é um cep válido usando regex

        var padrãoDoCep = "[0-9]{5}-[0-9]{3}";

        if (!Regex.IsMatch(cep, padrãoDoCep))
        {
            throw new ArgumentException("O cep passado é inválido! Ele deve corresponder" +
                                             "ao formato XXXXX-XXX,"
                                            + " em que X é um número de 0 a 9!");
        }


        // 3. Recuperar os dados do cep na api dos correios
        //    Se o cep não existir, lançar exceção

        //Será usada para criar um objeto do tipo endereco
        string jsonFromBody;

        using (var httpClient = new HttpClient())
        {

            try
            {
                var response = await httpClient.GetAsync(url);

                jsonFromBody = await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException)
            {
                throw;
            }

            //Como a api dos correios não retorna StatusCode diferente de 200 mas apenas um json
            //com um campo erro, foi necessário fazer isso
            if (jsonFromBody.Contains("erro"))
            {
                throw new KeyNotFoundException($"O CEP {cep} não existe!");
            }

            jsonFromBody = jsonFromBody.Replace("-", ""); //Para tirar o hífen do cep que não tem no banco de dados

            //Desserialização do json
            DataContractJsonSerializer serializador = new DataContractJsonSerializer(typeof(Endereco));
            MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(jsonFromBody));

            endereco = (Endereco?)serializador.ReadObject(memoryStream);

            if (endereco is null)
            {
                //Pouco provável, mas bom garantir
                throw new JsonException("Erro interno de processamento!");
            }

        }

        // 4. Validar se o CEP já não foi cadastrado no banco de dados

        if (_enderecoRepository.getEnderecoByCep(cep) is not null)
        {
            throw new DuplicateNameException($"O CEP {cep} já está registrado no banco de dados!");
        }

        // 5. Se passou em todos os testes, tenta armazenar os dados do cep no banco de dados

        try
        {
            _enderecoRepository.addEndereco(endereco);
        }
        catch (SqlException)
        {
            throw;
        }

        Console.WriteLine("Os dados: \n" + jsonFromBody + "\nforam armazenados no banco de dados!");

        return new RegistrationCepResponse(200, $"Os dados do cep {cep} foram registrados no banco de dados!");

    }
}