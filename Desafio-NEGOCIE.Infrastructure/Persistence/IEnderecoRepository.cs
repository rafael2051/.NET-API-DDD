using DesafioNEGOCIE.Domain.Entities;

namespace DesafioNEGOCIE.Infrastructure.Persistence;

public interface IEnderecoRepository
{
    Endereco? getEnderecoByCep(string cep);

    void addEndereco(Endereco endereco);
}