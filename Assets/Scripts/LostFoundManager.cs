using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.UI;

public class LostFoundManager : MonoBehaviour
{
    public LostFoundStorage Storage;
    public CustomerManager CustomerControl;

    public TextAsset CharadeJSON;

    public Transform BoardsTransform;

    public Text RiddleLabel;
    public AudioSource RightSource;
    public AudioSource WrongSource;
    
    private List<Charade> _riddles;

    private CustomerBehaviour _currentCustomer;


    public void Start()
    {
        StartLevel();
    }

    private void StartLevel()
    {
        CharadesDAO dao = CharadesDAO.Instance;
        dao.LoadCharadeData(CharadeJSON);

        List<string> items = dao.GenerateRandomItensList(12);
        _riddles = dao.GenerateRandomRiddlesList(items);

        Storage.SetStorage(items);
        Storage.ItemDropResolver = ProcessItemDrop;

        CustomerControl.N = 12;
        CustomerControl.StartLevel();
    }

    public void SetCustomerRiddle(CustomerBehaviour customer)
    {
        int index = UnityEngine.Random.Range(0,_riddles.Count);
        Charade riddle = _riddles[index];
        _riddles.RemoveAt(index);
        customer.Riddle = riddle;
    }

    private void OnRightAnswer(LostFoundItem lfItem, CustomerBehaviour customer)
    {
        SetRiddleLabel(null);
        RightSource.Play();
        Debug.Log("right answer");
        Storage.RemoveItem(lfItem, true);
        CustomerControl.ExitCustomer(customer);
    }

    private void OnWrongAnswer(LostFoundItem lfItem, CustomerBehaviour customer)
    {
        SetRiddleLabel(null);
        WrongSource.Play();
        Debug.Log("wrong answer");
        CustomerControl.ExitCustomer(customer);
    }

    public void OnOverwait(CustomerBehaviour customer)
    {
        WrongSource.Play();
        if (_currentCustomer == customer)
            SetRiddleLabel(null);
    }

    public void ProcessItemDrop(LostFoundItem lfItem, Action rejectCallback)
    {
        Vector3 pos = lfItem.transform.position;
        if (pos.y > BoardsTransform.position.y) rejectCallback();
        else
        {
            int index = 0;
            int bestIndex = -1;
            float bestDist = 1000000f;
            float dist;
            foreach(Transform t in BoardsTransform.Cast<Transform>().OrderBy(t => t.name))
            {
                dist = Vector3.Distance(t.position, pos);
                if (dist<bestDist)
                {
                    bestDist = dist;
                    bestIndex = index;
                }
                index++;
            }
            CustomerBehaviour target = CustomerControl.GetCustomerAt(bestIndex);
            if (target == null) //no customer there
                rejectCallback();
            else if (!target.Riddle.answer.Equals(lfItem.Item))
            {
                //Wrong answer
                rejectCallback();
                OnWrongAnswer(lfItem, target);
            }
            else
            {
                //Right answer
                OnRightAnswer(lfItem, target);
            }
        }
    }

    private void SetRiddleLabel(CustomerBehaviour customer)
    {
        if(_currentCustomer != null && _currentCustomer.gameObject != null)
        {
            _currentCustomer.ExcMark.enabled = false;
        }
        _currentCustomer = customer;
        if (_currentCustomer)
        {
            RiddleLabel.text = customer.Riddle.charade;
            _currentCustomer.ExcMark.enabled = true;
            Debug.Log(customer.Riddle.answer);
        }
        else
        {
            RiddleLabel.text = "";
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
            if (hit.collider != null)
            {
                CustomerBehaviour customer = hit.collider.GetComponent<CustomerBehaviour>();
                if (customer.IsWaiting)
                {
                    SetRiddleLabel(customer);
                }
            }
        }
    }
}
