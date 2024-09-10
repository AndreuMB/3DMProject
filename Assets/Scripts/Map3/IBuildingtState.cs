using UnityEngine;

public interface IBuildingtState
{
    void EndState(bool secondaryIndicator);
    void OnAction(Vector3Int gridPosition);
    void UpdateState(Vector3Int gridPosition, Vector3 gridPositionFloat);
}