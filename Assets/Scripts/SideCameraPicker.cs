using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SideCameraPicker : MonoBehaviour, IPointerClickHandler
{
    public Camera camera;
    public RectTransform textureTransform;

    public void OnPointerClick(PointerEventData eventData)
    {
        Vector2 localClick = new Vector2(eventData.position.x - (textureTransform.position.x + textureTransform.rect.xMin), eventData.position.y - (textureTransform.position.y + textureTransform.rect.yMin));
        //RectTransformUtility.ScreenPointToLocalPointInRectangle(textureTransform, eventData.position, Camera.main, out localClick);
        //localClick *= new Vector2(1f, -1f);

        localClick /= new Vector2(textureTransform.rect.width, textureTransform.rect.height);

        Ray ray = camera.ViewportPointToRay(localClick);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1000f))
        {
            hit.collider.GetComponent<MeshRenderer>().material.color = new Color(220f/255f, 179f/255f, 250f/255f);
        }
    }
}
