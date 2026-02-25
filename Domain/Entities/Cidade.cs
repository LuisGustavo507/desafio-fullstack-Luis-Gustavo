namespace Domain.Entities;

public class Cidade
{
    public int CidadeId { get; private set; }
    public string Nome { get; private set; } = string.Empty;
    public string Pais { get; private set; } = string.Empty;

    private Cidade() { }

    public Cidade(string nome, string pais)
    {
        Nome = nome;
        Pais = pais;
    }
}
