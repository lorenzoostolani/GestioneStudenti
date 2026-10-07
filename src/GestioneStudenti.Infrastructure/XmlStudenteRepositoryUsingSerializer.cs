using GestioneStudenti.Application;
using GestioneStudenti.Domain;
using System.Xml.Serialization;
using static System.Net.Mime.MediaTypeNames;

public class XmlStudenteRepositoryUsingSerializer : IStudenteRepository
{
    private readonly string _percorso;

    public XmlStudenteRepositoryUsingSerializer(string percorso)
    {
        _percorso = percorso;
    }

    public List<Studente> GetAll()
    {
        return Load();
    }

    public Studente? GetById(int id)
    {
        List<Studente> studenti = Load();

        int indice = CercaIndice(studenti, id);

        if (indice < 0)
            return null;

        return studenti[indice];
    }

    public void Add(Studente studente)
    {
        List<Studente> studenti = Load();

        if (CercaIndice(studenti, studente.Id) >= 0)
            throw new InvalidOperationException(
                $"Esiste già uno studente con id {studente.Id}.");

        studenti.Add(studente);

        Save(studenti);
    }

    public void Update(Studente studente)
    {
        List<Studente> studenti = Load();

        int indice = CercaIndice(studenti, studente.Id);

        if (indice < 0)
            throw new InvalidOperationException(
                $"Studente con id {studente.Id} non trovato.");

        studenti[indice] = studente;

        Save(studenti);
    }

    public bool Delete(int id)
    {
        List<Studente> studenti = Load();

        int indice = CercaIndice(studenti, id);

        if (indice < 0)
            return false;

        studenti.RemoveAt(indice);

        Save(studenti);

        return true;
    }


    private List<Studente> Load()
    {
        var studenti = new List<Studente>();

        if (!File.Exists(_percorso))
            return studenti;

        XmlSerializer serializer = new XmlSerializer(typeof(List<StudenteDto>));

        using FileStream stream = File.OpenRead(_percorso);

        List<StudenteDto>? dati = serializer.Deserialize(stream) as List<StudenteDto>;

        if (dati == null)
            return studenti;

        // DTO -> entità di dominio
        foreach (StudenteDto d in dati)
        {
            studenti.Add(new Studente(d.Id, d.Nome, d.Cognome, d.Classe));
        }

        return studenti;
    }

    private void Save(List<Studente> studenti)
    {
        // entità di dominio -> DTO
        var dati = new List<StudenteDto>();

        foreach (Studente s in studenti)
        {
            dati.Add(new StudenteDto(s.Id, s.Nome, s.Cognome, s.Classe));
        }

        XmlSerializer serializer = new XmlSerializer(typeof(List<StudenteDto>));

        using FileStream stream = File.Create(_percorso);

        serializer.Serialize(stream, dati);
    }

    private static int CercaIndice(List<Studente> studenti, int id)
    {
        for (int i = 0; i < studenti.Count; i++)
        {
            if (studenti[i].Id == id)
                return i;
        }

        return -1;
    }
}

internal sealed record StudenteDto(int Id, string Nome, string Cognome, string Classe);

