using DesafioNEGOCIE.Domain.Entities;
using Dapper;
using Npgsql;
using Microsoft.Extensions.Options;

namespace DesafioNEGOCIE.Infrastructure.Persistence;

public class EnderecoRepository : IEnderecoRepository
{

    private readonly ConnectionSettings _connectionSettings;

    public EnderecoRepository(IOptions<ConnectionSettings> connectionOptions)
    {
        _connectionSettings = connectionOptions.Value;
    }

    public void addEndereco(Endereco endereco)
    {

        var sqlQuery =
        "INSERT INTO endereco (cep, logradouro, complemento, bairro, localidade, uf, ibge, gia, ddd, siafi) " +

        $"VALUES (@cep," +
        $"@logradouro," +
        $"@complemento," +
        $"@bairro," +
        $"@localidade," +
        $"@uf," +
        $"@ibge," +
        $"@gia," +
        $"@ddd," +
        $"@siafi" +
        ");";

        try
        {
            using (var connection = new NpgsqlConnection(_connectionSettings.ConnectionString))
            {
                connection.Open();
                var rowsAffected = connection.Execute(sqlQuery, endereco);
                Console.WriteLine($"{rowsAffected} row(s) inserted.");

            }
        }
        catch (NpgsqlException e)
        {
            Console.WriteLine("Ocorreu uma exceção ao tentar realizar a inserção de um registro no banco de dados. Motivo: " + e.Message);
            throw;
        }
    }

    public Endereco? getEnderecoByCep(string cep)
    {

        //Pelo fato de que o campo cep no banco de dados é um VARCHAR(8)
        var cepConsulta = cep.Replace("-", "");

        var sqlQuery = "SELECT * from endereco where cep=@cep";

        try
        {
            using (var connection = new NpgsqlConnection(_connectionSettings.ConnectionString))
            {
                connection.Open();

                var endereco = connection.QuerySingleOrDefault<Endereco>(sqlQuery, new { cep = cepConsulta });

                //Para retornar um cep no formato XXXXX-XXX para o cliente
                if (endereco is not null)
                {
                    endereco.cep = cep;
                }

                return endereco;
            }
        }
        catch (NpgsqlException e)
        {
            Console.WriteLine("Ocorreu uma exceção ao recuperar um registro do bando de dados. Motivo: " + e.Message);
            throw;
        }

    }
}