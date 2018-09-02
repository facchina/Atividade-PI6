using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using SimulationEnum;
public class Client {

    /* Toda vez que é escrito new Random (), ele é inicializado. 
     * Isso significa que em um loop apertado você obtém o mesmo valor várias vezes. 
     * Você deve manter uma única instância aleatória e continuar usando o Next na mesma instância.
     * */

    private static System.Random rnd = new System.Random();
    private Simulation simulation = Simulation.instance;
    private readonly String request;
    private String currentRequest;
    private int _id;
    /// <summary>
    /// informa quando o cliente ja comecou a ser atendido
    /// </summary>
    private bool beingAttended = false;
    //variables for the report
    /// <summary>
    /// horário que chegou na loja
    /// </summary>
    private int _arrivedStoreTime;
    /// <summary>
    /// horário que saiu da loja
    /// </summary>
    private int _leftStoreTime;
    /// <summary>
    /// horario que começou a ser atendido
    /// </summary>
    private int _attendanceStartTime; 


    String generateRequest()
    {
        int index;
        if (simulation.TypeOfSimulation == SimulationType.randomValues)
        {
            index = rnd.Next(0, Constants.request.Length);
            return Constants.request[index];
        }
        else {
            return Constants.deterministicRequests[_id];
        }
    }

    public int Id
    {
        get { return _id; }
        set { _id = value; }
    }

    public Client(int id)
    {
        this.Id = id;
        this.ArrivedStoreTime = simulation.tick;
        request = generateRequest();
        this.currentRequest = request;
    }

    public String CurrentRequest
    {
        set { currentRequest = value; }
        get { return currentRequest; }
    }

    public int LeftStoreTime
    {
        get
        {
            return _leftStoreTime;
        }

        set
        {
            _leftStoreTime = value;
        }
    }

    public bool BeingAttended
    {
        get
        {
            return beingAttended;
        }

        set
        {
            beingAttended = value;
        }
    }

    public int AttendanceStartTime
    {
        get
        {
            return _attendanceStartTime;
        }

        set
        {
            _attendanceStartTime = value;
        }
    }

    public int ArrivedStoreTime
    {
        get
        {
            return _arrivedStoreTime;
        }

        set
        {
            _arrivedStoreTime = value;
        }
    }

    public string Request
    {
        get
        {
            return request;
        }
    }
}



