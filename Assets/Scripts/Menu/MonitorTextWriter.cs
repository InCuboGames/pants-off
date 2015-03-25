using UnityEngine;
using System.Collections;

public class MonitorTextWriter : MonoBehaviour {

    TextMesh textMesh;
    int currentPosition = 0;
    [SerializeField] float delay;  // 10 characters per sec.
    string text = "";

    [SerializeField]
    AudioClip keystrokeSFX;

    AudioSource audioSource;

    void Start()
    {
        textMesh = GetComponent<TextMesh>();
        audioSource = GetComponent<AudioSource>();
        ChangeText("");
    }

    public void ChangeText(string aText) {
        if(textMesh)
        {
            //textMesh.text = "C:\\ ";
			textMesh.text = "";
            currentPosition = textMesh.text.Length;
            text = textMesh.text + aText;

            StartCoroutine(WriteString());
        }
    }
    
    IEnumerator WriteString()
    {
        yield return new WaitForSeconds(delay);

        if (currentPosition < text.Length)
        {
            audioSource.PlayOneShot(keystrokeSFX);
            textMesh.text += text[currentPosition++];
            StartCoroutine(WriteString());
        }
        else
        {
            yield return null;
        }
    }
}
