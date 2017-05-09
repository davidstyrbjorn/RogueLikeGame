using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class IntroNotes : MonoBehaviour {

    public RectTransform parentTransform;
    public RectTransform firstPage;
    public RectTransform[] pages;
    private int pageIndex = -1;

    public void NextPage()
    {
        if(pageIndex == pages.Length-1)
        {
            Skip();
            return;
        }

        firstPage.gameObject.SetActive(false);
        if (pageIndex != -1)
            pages[pageIndex].gameObject.SetActive(false);

        pageIndex++;
        pages[pageIndex].gameObject.SetActive(true);
    }

    public void Skip()
    {
        pageIndex = -1;
        for (int i = 0; i < pages.Length; i++)
            pages[i].gameObject.SetActive(false);
        firstPage.gameObject.SetActive(true);
        parentTransform.gameObject.SetActive(false);
    }
}
