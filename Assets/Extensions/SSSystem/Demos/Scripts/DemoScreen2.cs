using UnityEngine;
using System.Collections;

public class DemoScreen2 : SSController
{
    public void OnPlayButtonTap()
    {
        SSSceneManager.Instance.ShowLoading();

        StartCoroutine(SimulationLoadingTime());
    }

    private IEnumerator SimulationLoadingTime()
    {
        yield return new WaitForSeconds(0.1f);

        SSSceneManager.Instance.Screen("DemoPlay", null,
            (ctrl) =>
            {
                SSSceneManager.Instance.HideLoading();
            }
        );
    }

    public void OnBackButtonTap()
    {
        SSSceneManager.Instance.Close();
    }
}
