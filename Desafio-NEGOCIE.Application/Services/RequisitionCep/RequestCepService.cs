using System.Text.RegularExpressions;
using DesafioNEGOCIE.Infrastructure.Persistence;

namespace DesafioNEGOCIE.Application.Services.RequisitionCep;

public class RequisitionCepService : IRequisitionCepService
{

    private readonly IEnderecoRepository _enderecoRepository;

    public RequisitionCepService(IEnderecoRepository enderecoRepository)
    {
        _enderecoRepository = enderecoRepository;
    }

    public RequisitionCepResponse RequestCep(string cep)
    {

        // 1. Validar de se é um cep válido usando regex

        var padrãoDoCep = @"^(\d{5}-\d{3})$";

        if (!Regex.IsMatch(cep, padrãoDoCep))
        {
            throw new ArgumentException("O cep passado é inválido!");
        }

        var endereco = _enderecoRepository.getEnderecoByCep(cep);

        if (endereco is null)
        {
            throw new KeyNotFoundException($"O cep {cep} não está registrado no banco de dados!");
        }


        return new RequisitionCepResponse(
            200,
            endereco
        );
    }
}