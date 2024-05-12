namespace DesafioNEGOCIE.Application.Services.RegistrationCep;

public interface IRegistrationCepService
{
    public Task<RegistrationCepResponse> RegisterCep(string cep);
}