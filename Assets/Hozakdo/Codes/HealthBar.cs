using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private Slider slider;
    private Unit unit;    
    private Canvas canvas;
    
    private void Awake()
    {
        slider = GetComponent<Slider>();
    }
    
    private void Start()
    {
        unit = GetComponentInParent<Unit>();
        canvas = GetComponentInParent<Canvas>();
        
        if (canvas != null && canvas.renderMode != RenderMode.WorldSpace)
        {
            Debug.LogWarning($"{gameObject.name}의 Canvas가 World Space로 설정되지 않았습니다!");
        }
        UpdateHealthBar();
    }
    
    private void LateUpdate()
    {
        UpdateHealthBar();
    }
    
    private void UpdateHealthBar()
    {
        slider.value = unit.health / unit.maxHealth;
    }
}

