using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class StartButtonFlicker : MonoBehaviour
{
    [SerializeField]
    GameObject textMeshPro;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FlickerProcess());
    }

    IEnumerator FlickerProcess()
    {
        while (true) {
            for (int i = 0; i < Random.Range(2, 6); i += 1)
            {
                this.textMeshPro.SetActive(false);
                yield return new WaitForSeconds(0.05f);
                this.textMeshPro.SetActive(true);
                yield return new WaitForSeconds(0.1f);
            }
            yield return new WaitForSeconds(Random.Range(4f, 8f));
        }
    }
}
