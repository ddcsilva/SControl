namespace SControl.Business.Models;

public class Curso : Entity
{
    public string Nome { get; set; }
    public int DuracaoPorSemestre { get; set; }
    public string Modalidade { get; set; }
    public bool Ativo { get; set; }
}
