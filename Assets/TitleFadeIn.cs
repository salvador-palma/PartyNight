using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
public class TitleFadeIn : MonoBehaviour
{

    private void Start() {

        GetComponent<TextMeshProUGUI>().text = SceneManager.GetActiveScene().name;
    }
}
