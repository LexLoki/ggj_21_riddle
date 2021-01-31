using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneNavigator : MonoBehaviour
{
    public void GoToScene(string name)
    {
        SceneManager.LoadScene(name);
    }
}
