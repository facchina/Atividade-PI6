using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Employee : MonoBehaviour {
    private static System.Random rnd = new System.Random();
    private Simulation simulation = Simulation.instance;
    public bool avaiable;

    public enum Function{ donuts, cafe, donutsEcafe }
    public string _role;
    public string Role
    {
        get { return _role; }
        set { _role = value; }
    }

    public void Start() {

        avaiable = true;
    }

    public IEnumerator attending(Client c, float time)
    {
        avaiable = false;
        //caso seja o inicio do atendimento do cliente being attended eh setado para true
        if (!c.BeingAttended) {
            c.BeingAttended = true;
            if (!simulation.ClientsServed.Contains(c))
            {
                c.AttendanceStartTime = simulation.tick;
                simulation.ClientsServed.Add(c);
            }
            else {
                Debug.Log("nao era pra isso ter aconteceido! o_o");
            }
        }

        while (time > 0)
        {
            yield return new WaitForSeconds(1.0f * Constants.velocity);
            time--;
        }

        c.CurrentRequest = c.CurrentRequest.Remove(0, 1);
        if (c.CurrentRequest.Length == 0) {
            c.LeftStoreTime = simulation.tick;
            c.BeingAttended = false;
            //Debug.Log("o cliente " + c.Id + " foi embora");
        }
        else {
            //Debug.Log("o cliente " + c.Id + "foi para fila de tosa");
            simulation.PreferredClients.Enqueue(c);
        }
        StartCoroutine(goingToNextClient());
    }

    //aguarda um segundo para entao estar livre para atender outro cliente
    public IEnumerator goingToNextClient() {
        yield return new WaitForSeconds(1.0f * Constants.velocity);
        //Debug.Log("Procurando um proximo cliente da fila de espera");
        avaiable = true;

    }

    //verifica se pode pegar o pedido
    public bool tookRequest(Client client)
    {
        if (!avaiable) {
            
            return false;
        }

        char primeiroPedido = client.CurrentRequest.FirstOrDefault();
        if (_role.Contains(primeiroPedido))
        {
            float t = timeRequest(primeiroPedido.ToString());
            StartCoroutine(attending(client, t));
            return true;
        }
        else {
            return false;
        }
    }

    private float timeRequest(string request) {
        float time = 0;
        //print("request: " + request);
        switch (request)
        {
            case "A":
                time = 3;
                break;
            case "B":
                time = 5;
                break;
            default:
                break;
        }

        if (time == 0) {
            Debug.Log("error ao calcular tempo de preparo do produto");
        }
        return time;
    }

}
