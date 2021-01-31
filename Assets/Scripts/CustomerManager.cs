using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CustomerManager : MonoBehaviour
{

    [System.Serializable]
    public class CustomerEvent : UnityEvent<CustomerBehaviour> { }

    public int N;
    public float SpawnInterval;
    public Transform SpawnPoint;
    public Transform[] EntryPoints;

    public GameObject CustomerPrefab;

    public CustomerEvent OnCustomerSpawn;
    //public UnityEvent<CustomerBehaviour> OnCustomerOverwait;
    public CustomerEvent OnCustomerOverwait;

    private bool[] _busy;
    private int EntryCap { get { return EntryPoints.Length; } }
    private CustomerBehaviour[] _customerPerEntry;
    private int _nLeft;
    private int _nOut;

    public void StartLevel()
    {
        _busy = new bool[EntryPoints.Length];
        _customerPerEntry = new CustomerBehaviour[EntryPoints.Length];
        for (int i = 0; i < EntryPoints.Length; i++) _busy[i] = false;
        _nLeft = N;
        _nOut = N;
        StartCoroutine(LevelRoutine());
    }

    public CustomerBehaviour GetCustomerAt(int blockIndex)
    {
        Debug.Log(blockIndex);
        return _customerPerEntry[blockIndex];
    }

    IEnumerator LevelRoutine()
    {
        SpawnCustomer();
        for (int nLeft = N - 1; nLeft > 0; nLeft--)
        {
            yield return new WaitForSeconds(SpawnInterval);
            SpawnCustomer();
        }
    }

    private void SpawnCustomer()
    {
        List<int> freeIndexes = new List<int>();
        for (int i = 0; i < EntryCap; i++)
            if (!_busy[i]) freeIndexes.Add(i);
        int spawnIndex = freeIndexes[Random.Range(0, freeIndexes.Count)];
        _busy[spawnIndex] = true;
        Transform spawnT = EntryPoints[spawnIndex];
        GameObject customer = Instantiate(CustomerPrefab, SpawnPoint.position, Quaternion.identity, this.transform);
        SetupCustomer(customer, spawnIndex);
    }

    private void HandleCustomerReach(CustomerBehaviour customer)
    {
        customer.StartWait(t =>
        {
            OnCustomerOverwait.Invoke(t);
            ExitCustomer(t);
        });
    }

    public void ExitCustomer(CustomerBehaviour customer)
    {
        int entryIndex = customer.ID;
        _busy[entryIndex] = false;
        _customerPerEntry[entryIndex] = null;
        customer.SetMovement(SpawnPoint, HandleCustomerOut);
    }

    private void HandleCustomerOut(CustomerBehaviour customer)
    {
        GameObject.Destroy(customer.gameObject);
        if (_nOut-- <= 0)
        {
            //game end
        }
    }

    private void SetupCustomer(GameObject customerObject, int entryIndex)
    {
        CustomerBehaviour custBehaviour = customerObject.GetComponent<CustomerBehaviour>();
        _customerPerEntry[entryIndex] = custBehaviour;
        custBehaviour.ID = entryIndex;
        custBehaviour.SetMovement(EntryPoints[entryIndex], HandleCustomerReach);
        OnCustomerSpawn.Invoke(custBehaviour);
    }

}
