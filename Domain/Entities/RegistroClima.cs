namespace Domain.Entities;

public class RegistroClima
{
    public int RegistroClimaId { get; private set; }
    public decimal Latitude { get; private set; }
    public decimal Longitude { get; private set; }
    public double Temperatura { get; private set; }
    public double TemperaturaMin { get; private set; }
    public double TemperaturaMax { get; private set; }
    public string Condicao { get; private set; } = string.Empty;
    public DateTime DataHoraRegistro { get; private set; }
    public int? CidadeId { get; private set; }
    public virtual Cidade? Cidade { get; private set; }

    private RegistroClima() { }

    public RegistroClima(
        decimal latitude, 
        decimal longitude, 
        double temperatura, 
        double temperaturaMin, 
        double temperaturaMax, 
        string condicao, 
        DateTime dataHora, 
        Cidade? cidade)
    {
        Latitude = latitude;
        Longitude = longitude;
        Temperatura = temperatura;
        TemperaturaMin = temperaturaMin;
        TemperaturaMax = temperaturaMax;
        Condicao = condicao;
        DataHoraRegistro = dataHora;
        Cidade = cidade;
        CidadeId = cidade?.CidadeId;
    }
}
