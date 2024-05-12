using DesafioNEGOCIE.Domain.Entities;

namespace DesafioNEGOCIE.Application.Services.RequisitionCep;
public record RequisitionCepResponse(

    int httpCode,

    Endereco endereco

);