using LABOFMUTANTAPI.Dto;
using LABOFMUTANTAPI.Entities;
namespace LABOFMUTANTAPI.Services.Interfaces;
public interface ISyncService
{
    Task<SyncResponseDto> Receive(string token, SyncRequestDto request, CancellationToken cancellation);
    Task<DeviceState> Get(string token, CancellationToken cancellation);
}
