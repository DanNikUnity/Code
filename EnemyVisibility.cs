using MenteBacata.ScivoloCharacterControllerDemo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class EnemyVisibility : MonoBehaviour
{
    public float fadeDuration = 1.0f;  // ������������ ��������� (� ��������)
    private bool visible = true;       // ����, �����������, ���� �� � ������ ������ �������� ���������

    public GameObject[] objectsToFade; // ������ ��������, � ������� ����� �������� ���������
    private List<Material> allMaterials = new List<Material>(); // ������ ���� ����������

    private List<Color> originalColors = new List<Color>();     // ������ �������� ������ ����������
    private List<Color> transparentColors = new List<Color>();  // ������ ���������� ������ ����������

    private Coroutine fadeCoroutine = null;
    private Transform player;

    private void Start()
    {
        // �������� ��� ��������� �� �������� � ��������� �� �������� � ���������� �����
        foreach (GameObject obj in objectsToFade)
        {
            Renderer renderer = obj.GetComponent<Renderer>();
            if (renderer != null)
            {
                Material[] mats = renderer.materials;
                foreach (Material mat in mats)
                {
                    allMaterials.Add(mat);
                    originalColors.Add(mat.color);
                    transparentColors.Add(new Color(mat.color.r, mat.color.g, mat.color.b, 0));
                }
            }
        }

        player = FindObjectOfType<SimpleCharacterController>().GetComponent<Transform>();

        // �������� ��������� ��������� ������������
        for (int i = 0; i < allMaterials.Count; i++)
        {
            allMaterials[i].color = transparentColors[i];
        }
        SetVisible(false);
    }

    public bool HasLineOfSight(Transform target)
    {
        Vector3 origin = transform.position;
        Vector3 direction = target.position - origin;
        RaycastHit hit;

        // ���������, ��� raycast ��������� ���������� ���� � ���������� ����������
        if (Physics.Raycast(origin, direction, out hit))
        {
            Debug.Log("Raycast hit: " + hit.transform.name);
            if (hit.transform == target)
            {
                return true;
            }
        }

        return false;
    }

    public void SetVisible(bool isVisible)
    {
        if (visible != isVisible)
        {
            visible = isVisible;
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }
            fadeCoroutine = StartCoroutine(FadeMaterials(isVisible));
        }
    }

    private IEnumerator FadeMaterials(bool fadeToOpaque)
    {
        float currentFadeTime = 0.0f;
        List<Color> startColors = new List<Color>();

        // ��������� ������� ����� ��� ���������
        for (int i = 0; i < allMaterials.Count; i++)
        {
            startColors.Add(allMaterials[i].color);
        }

        List<Color> targetColors = fadeToOpaque ? originalColors : transparentColors;

        while (currentFadeTime < fadeDuration)
        {
            currentFadeTime += Time.deltaTime;
            float t = currentFadeTime / fadeDuration;
            for (int i = 0; i < allMaterials.Count; i++)
            {
                allMaterials[i].color = Color.Lerp(startColors[i], targetColors[i], t);
            }
            yield return null;
        }

        // ���������, ��� �������� �������� �����������
        for (int i = 0; i < allMaterials.Count; i++)
        {
            allMaterials[i].color = targetColors[i];
        }
    }
}
