namespace LABOFMUTANTAPI.Entities;

public sealed class Dispositivo
{
    public string Id { get; set; } = "";
    public long UltimaSecuencia { get; set; }
    public DateTimeOffset ActualizadoUtc { get; set; }
}
public sealed class Perfil
{
    public string DispositivoId { get; set; } = "";
    public string Nombre { get; set; } = "";
}
public sealed class Preferencia
{
    public string DispositivoId { get; set; } = "";
    public bool Linterna { get; set; }
    public string PestanaGuia { get; set; } = "t1";
}
public sealed class Progreso
{
    public string DispositivoId { get; set; } = "";
    public long Visitas { get; set; }
    public long? PrimeraVisita { get; set; }
    public long? UltimaVisita { get; set; }
    public long VecesDetectado { get; set; }
    public long MejorSigilo { get; set; }
    public string TeclasProbadasJson { get; set; } = "[]";
}
public sealed class NotaDiario
{
    public string DispositivoId { get; set; } = "";
    public long IdLocal { get; set; }
    public string Texto { get; set; } = "";
    public long CreadaUnixMs { get; set; }
}
