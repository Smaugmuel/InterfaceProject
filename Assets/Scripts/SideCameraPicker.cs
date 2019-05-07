using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SideCameraPicker : MonoBehaviour, IPointerClickHandler
{
    public Camera camera;

    public Canvas canvas;
    public RectTransform textureTransform;

    public void OnPointerClick(PointerEventData eventData)
    {
        float realWidth = textureTransform.rect.width * canvas.scaleFactor;
        float realHegiht = textureTransform.rect.height * canvas.scaleFactor;

        // Calculate local point of click
        Vector2 localClick = new Vector2(eventData.position.x - (textureTransform.position.x - (realWidth/2f)), eventData.position.y - (textureTransform.position.y - (realHegiht/2f)));

        // Convert x and y to [0f;1f]
        localClick /= new Vector2(realWidth, realHegiht);

        // Shoot ray from side camera
        Ray ray = camera.ViewportPointToRay(localClick);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1000f))
        {
            hit.collider.GetComponent<MeshRenderer>().material.color = new Color(220f/255f, 179f/255f, 250f/255f);
        }
    }
}
