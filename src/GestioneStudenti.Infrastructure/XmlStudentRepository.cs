using GestioneStudenti.Application;
using GestioneStudenti.Domain;
using System.Xml;

public class XmlStudenteRepository : IStudenteRepository
{
    private readonly string _percorso;

    public XmlStudenteRepository(string percorso)
    {
        _percorso = percorso;
    }

    //metodi dell'interfaccia
    public List<Studente> GetAll()
    {
        return Load();
    }

    public Studente GetById(int id)
    {
        List<Studente> studenti = Load();

        int indice = CercaIndice(studenti, id);

        if (indice < 0)
            return null;

        return studenti[indice];
    }


    public void Add(Studente studente)
    {
        XmlDocument documento = new XmlDocument();

        //carichiamo il file
        if (File.Exists(_percorso))
            documento.Load(_percorso);
        else
        {
            XmlNode root = documento.CreateElement("Studenti");
            documento.AppendChild(root);
        }

        XmlNode rootNode = documento.DocumentElement;

        // controllaimo se esiste gia lo studente
        XmlNodeList studenti = rootNode.ChildNodes;

        foreach (XmlNode nodo in studenti)
        {
            int studentId = int.Parse(nodo.Attributes["id"].Value);

            if (studentId == studente.Id)
                throw new InvalidOperationException(
                    $"Esiste già uno studente con id {studente.Id}.");
        }

        //creazione studente
        XmlNode nuovoStudente =
            documento.CreateElement("Studente");

        XmlAttribute id = documento.CreateAttribute("id");
        id.Value = studente.Id.ToString();

        //Collegamento dell'attributo id_libro al Nodo nuovo_libro
        nuovoStudente.Attributes.Append(id);

        //nome
        XmlNode nome = documento.CreateElement("Nome");
        nome.InnerText = studente.Nome;
        nuovoStudente.AppendChild(nome);

        //cognome
        XmlNode cognome = documento.CreateElement("Cognome");
        cognome.InnerText = studente.Cognome;
        nuovoStudente.AppendChild(cognome);

        //classe
        XmlNode classe = documento.CreateElement("Classe");
        classe.InnerText = studente.Classe;
        nuovoStudente.AppendChild(classe);

        // aggiongiamo lo studente alla root
        rootNode.AppendChild(nuovoStudente);

        // salviamo
        documento.Save(_percorso);
    }

    public void Update(Studente studente)
    {
        if (!File.Exists(_percorso))
            throw new InvalidOperationException(
                $"Studente con id {studente.Id} non trovato.");

        XmlDocument documento = new XmlDocument();
        documento.Load(_percorso);

        XmlNode root = documento.DocumentElement;

        foreach (XmlNode nodoStudente in root.ChildNodes)
        {
            int id = int.Parse(nodoStudente.Attributes["id"].Value);

            if (id == studente.Id)
            {
                XmlNodeList elementi = nodoStudente.ChildNodes;

                elementi[0].InnerText = studente.Nome;
                elementi[1].InnerText = studente.Cognome;
                elementi[2].InnerText = studente.Classe;

                documento.Save(_percorso);
                return;
            }
        }

        throw new InvalidOperationException(
            $"Studente con id {studente.Id} non trovato.");
    }

    public bool Delete(int id)
    {
        if (!File.Exists(_percorso))
            return false;

        XmlDocument documento = new XmlDocument();
        documento.Load(_percorso);

        XmlNode root = documento.DocumentElement;
        if (root == null)
            throw new InvalidOperationException(
                $"Studente con id {id} non trovato.");

        foreach (XmlNode nodoStudente in root.ChildNodes)
        {
            int idStudente = int.Parse(
                nodoStudente.Attributes["id"].Value);

            if (idStudente == id)
            {
                root.RemoveChild(nodoStudente);

                documento.Save(_percorso);

                return true;
            }
        }

        return false;
    }

    //metodi per leggere e scrivere nell'xml

    private List<Studente> Load()
    {
        var studenti = new List<Studente>();

        if (!File.Exists(_percorso))
            return studenti;

        XmlDocument documento = new XmlDocument();
        documento.Load(_percorso);

        XmlNode root = documento.DocumentElement;

        if (root == null)
            return studenti;

        // Recupero tutti i nodi Studente
        XmlNodeList nodiStudenti = root.ChildNodes;

        foreach (XmlNode nodoStudente in nodiStudenti)
        {
            int id = int.Parse(nodoStudente.Attributes["id"].Value);

            XmlNodeList elementi = nodoStudente.ChildNodes;

            string nome = elementi[0].InnerText;
            string cognome = elementi[1].InnerText;
            string classe = elementi[2].InnerText;

            studenti.Add(
                new Studente(id, nome, cognome, classe));
        }

        return studenti;
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
