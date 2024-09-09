using UnityEngine;

[CreateAssetMenu(fileName = "GameMaterial", menuName = "Custom/GameMaterial")]
public class GameMaterialSO : ScriptableObject
{
    public GameMaterialsEnum materialName;
    public GameMaterialTypesEnum type;
}
