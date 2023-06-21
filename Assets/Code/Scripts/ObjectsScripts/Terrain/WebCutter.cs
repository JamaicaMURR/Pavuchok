using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebCutter : WebAnchorInteractor
{
    public override void Income(WebAnchor webAnchor) => webAnchor.AnchorCut();
    public override void Recome(WebAnchor webAnchor) => Income(webAnchor);
    public override void Outcome() { }
}
