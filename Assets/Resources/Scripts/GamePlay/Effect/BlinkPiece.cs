using UnityEngine;
using System.Collections;

public class BlinkPiece : TimeEffect {
    private Color32 color;
    private SpriteRenderer render;
    public void Init(Piece piece, float time)
    {
        render = piece.GetComponent<SpriteRenderer>();
        color = render.color;
        progress = new Counter(time);
        TimerControl.Instance.effects += OnBlinkPieceUpdate;

    }
	
	// Update is called once per frame
    public void OnBlinkPieceUpdate()
    {
        progress.Tick(Time.deltaTime);
        if (progress.Expired())
        {
            TimerControl.Instance.effects -= OnBlinkPieceUpdate;
        }
        else
        {
            float percent = Mathf.Sin(Mathf.PI*progress.percent);
            render.color = new Color32(Utility.LerpColorChannel(color.r, 255, percent), Utility.LerpColorChannel(color.g, 255, percent), Utility.LerpColorChannel(color.b, 255, percent), 255);
        }
	}
}
