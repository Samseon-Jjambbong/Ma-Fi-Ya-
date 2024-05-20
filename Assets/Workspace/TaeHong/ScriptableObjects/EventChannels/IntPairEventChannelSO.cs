using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Events/Int Pair Event Channel", fileName = "IntPairEventChannel")]
public class IntPairEventChannelSO : GenericEventChannelSO<(int, int)>
{
}
