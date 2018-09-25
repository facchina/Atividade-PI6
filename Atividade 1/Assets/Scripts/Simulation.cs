using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;

using SimulationEnum;
static class Constants
{
    /// <summary>
    /// velocidade relativa de cada tick
    /// </summary>
    public const float velocity = 0.25f; 
    public static readonly string[] request = { "A", "B", "AB" };
    public static readonly string[] deterministicRequests = { "A",  "AB", "B",  "A",  "AB",
                                                              "B",  "A",  "B",  "A",  "AB",
                                                              "AB", "AB", "AB", "AB", "A",
                                                              "A",  "B",  "A",  "A",  "AB", 
                                                              "B",  "B",  "A",  "AB", "B",
                                                              "A",  "B",  "A",  "A",  "AB",
                                                              "B",  "B",  "B",  "B",  "A",
                                                              "AB", "A",  "AB", "AB", "B",
                                                              "AB", "AB", "B",  "A",  "A"
                                                            };   

}

namespace SimulationEnum {
    public enum SimulationType { deterministic, randomValues }
}
public class Simulation : MonoBehaviour
{
    public static Simulation instance;
    /// <summary>
    /// fila de clientes que chegaram na loja
    /// </summary>
    private Queue<Client> clients = new Queue<Client>();
    /// <summary>
    /// fila de clientes que estao esperando a proxima parte do seu pedido ser realizada
    /// </summary>
    private Queue<Client> _preferredClients = new Queue<Client>();
    /// <summary>
    /// lista de clientes que foram atendidos pela loja
    /// </summary>
    private List<Client> _clientsServed = new List<Client>();
    /// <summary>
    /// lista de funcionarios
    /// </summary>
    public List<GameObject> employees = new List<GameObject>();

    public int tick = 0;
    /// <summary>
    /// Numero de clientes que vao chegar na loja.
    /// Quando o modelo eh randomValues a qtd de clientes vai aumentando com o tempo
    /// Quando o modelo eh deterministico a quantidade eh sempre a mesma
    /// </summary>
    public int numberClients = 0;
    /// <summary>
    /// periodo em que aceitam receber pedidos de banho e tosa no petshop (6h x 60 ticks)
    /// </summary>
    private float workPeriod = 1 * 60;
    /// <summary>
    /// informa quando o pet fechou
    /// </summary>
    public bool workPeriodEnded = false;
    //defines the type of simulation
    private SimulationType typeOfSimulation = SimulationType.deterministic;

    private void Awake()
    {
        
        if (instance != null)
        {
            Destroy(gameObject);
        }else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        workPeriodEnded = false;
        if (typeOfSimulation == SimulationType.deterministic) {
            numberClients = Constants.deterministicRequests.Length;
        }

        employees.Add(createEmployee("A"));
        employees.Add(createEmployee("A"));
        employees.Add(createEmployee("AB"));
        employees.Add(createEmployee("AB"));
    }
    // Use this for initialization
    void Start()
    {
        StartCoroutine(clientsComing());
    }

    private void Update()
    {
        if (clients.Count > 0 || PreferredClients.Count > 0)
        {
            Client c;
            if (PreferredClients.Count > 0)
            {
                c = PreferredClients.First();
                buscaEmployeeDisponivel(c, true);
            }
            else
            {
                c = clients.First();
                buscaEmployeeDisponivel(c, false);
            }
        }
        else if (workPeriodEnded) {
            //Debug.Log("qtd de clientes atendidos no dia: " + ClientsServed.Count);
            generateRecord();
        }
    }
    public IEnumerator clientsComing()
    {
        int id = 0;
        while (true)
        {
            if (tick < workPeriod)
            {
                if (typeOfSimulation == SimulationType.randomValues || id < numberClients)
                {
                    clients.Enqueue(new Client(id++));
                }
            }
            else {
                workPeriodEnded = true;
            }
            yield return new WaitForSeconds(1.0f * Constants.velocity);
            tick++;
        }

        //Debug.Log("final tick: " + tick);
    }

    public void buscaEmployeeDisponivel(Client c, bool tosa)
    {
        foreach (var g in employees)
        {
            Employee e = g.GetComponent<Employee>();
            if (e.tookRequest(c) == true)
            {
                //Debug.Log("o cliente " + c.Id + " achou alguem disponivel para fazer o pedido");
                if (!tosa){
                    clients.Dequeue();
                }
                else {
                    PreferredClients.Dequeue();
                }
                break;
            }
        }
    }

    public void generateRecord() {
        using (StreamWriter writer = new StreamWriter("record.csv"))
        {
            writer.WriteLine("{0}, {1}, {2}, {3}, {4}, {5}, {6}", "Id", 
                "Solicitação", "Horário Chegada", "Horário Saída", "Duracao Atendimento", "Tempo na fila", "Tempo na loja");
            foreach (var c in _clientsServed)
            {
                float duracaoAtendimento = c.LeftStoreTime - c.AttendanceStartTime;
                float tempoFila = c.AttendanceStartTime - c.ArrivedStoreTime;
                float tempoDentroLoja = c.LeftStoreTime - c.ArrivedStoreTime; 

                writer.WriteLine("{0}, {1}, {2}, {3}, {4}, {5}, {6}", c.Id, c.Request, c.ArrivedStoreTime.ToString(), 
                    c.LeftStoreTime.ToString(), duracaoAtendimento.ToString(), tempoFila.ToString(), tempoDentroLoja.ToString());
            }
            
        }

        Debug.Log("arquivo gerado");
    }

    public GameObject createEmployee(string role)
    {
        GameObject e = new GameObject();
        e.AddComponent<Employee>().Role = role;

        return e;
    }

    public Queue<Client> PreferredClients
    {
        get
        {
            return _preferredClients;
        }
    }


    public SimulationType TypeOfSimulation
    {
        get
        {
            return typeOfSimulation;
        }
    }

    public int NumberClients
    {
        get
        {
            return numberClients;
        }

        set
        {
            numberClients = value;
        }
    }

    public List<Client> ClientsServed
    {
        get
        {
            return _clientsServed;
        }

        set
        {
            _clientsServed = value;
        }
    }
}

