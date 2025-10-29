using UnityEngine;

public interface IShovelFlowHandler
{
    void OnShovelFilled(ShovelDirt shovel);   // shovel dipped into pail
    void OnShovelDumped(ShovelDirt shovel);   // shovel emptied into bucket
    void OnBucketSnapped(GameObject bucket);
}

