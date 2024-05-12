using DesafioNEGOCIE.Domain.Entities;

namespace DesafioNEGOCIE.Application.Services.RegistrationCep;

public record RegistrationCepResponse(
    int httpCode,
    string mensagem);