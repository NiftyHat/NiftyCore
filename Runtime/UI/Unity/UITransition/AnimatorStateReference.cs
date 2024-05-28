
using UnityEngine;

[System.Serializable]
public class AnimatorStateReference
{
    [SerializeField] private string _stateName;
    [SerializeField] private string _layerName;

    public string StateName => _stateName;

    private int _stateID = -1;
    private int _layerID = -1;

    public bool TryGetStateID(out int stateId)
    {
        if (_stateID >= 0)
        {
            stateId = _stateID;
            return true;
        }

        if (!string.IsNullOrWhiteSpace(_stateName))
        {
            stateId = Animator.StringToHash(_stateName);
            return true;
        }
        stateId = -1;
        return false;
    }

    public bool TryGetLayerIndex(out int layerIndex, Animator animator)
    {
        if (_layerID >= 0)
        {
            layerIndex = _layerID;
            return true;
        }

        if (!string.IsNullOrWhiteSpace(_layerName))
        {
            _layerID = layerIndex = animator.GetLayerIndex(_layerName);
            return layerIndex >= 0;
        }
        layerIndex = -1;
        return false;
    }
}
