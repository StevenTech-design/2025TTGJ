using UnityEngine;

public class OutlineColorTrigger : MonoBehaviour
{
    public Transform player;
    public float triggerDistance = 3f;
    private Renderer rend;
    private MaterialPropertyBlock mpb;
    private bool isHighlighted = false;
    [Header("Outline HDR Color")]
    public Color outlineColor = new Color(5f, 5f, 2.3f, 1f); // 默认红色
    [Range(0f, 20f)] public float intensity = 1f; // 高亮强度;

    void Start()
    {
        // 自动查找玩家（有 "Player" 标签）
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        rend = GetComponent<Renderer>();
        mpb = new MaterialPropertyBlock();
    }

    void Update()
    {
        if (player == null) return;

        float dist = Vector3.Distance(player.position, transform.position);
        bool shouldHighlight = dist < triggerDistance;

        if (shouldHighlight != isHighlighted)
        {
            isHighlighted = shouldHighlight;
            UpdateOutlineColor(isHighlighted);
        }
    }

    void UpdateOutlineColor(bool active)
    {
        rend.GetPropertyBlock(mpb);
        Color finalColor = active ? outlineColor * intensity  // 高亮 HDR
                                       : new Color(0.3f, 0.3f, 0.3f, 1f); // 默认
        mpb.SetColor("_OutLineColor", finalColor);

        rend.SetPropertyBlock(mpb);
    }
}

