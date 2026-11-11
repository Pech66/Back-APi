namespace LABOFMUTANTAPI.Dto;
public sealed record SyncResponseDto(long[] Accepted, long LastSequence, DateTimeOffset SyncedAt);
