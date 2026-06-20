using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterProfile", menuName = "Profiles/Character")]
public class CharacterData : ScriptableObject
{
    // --- Movement Stats ---
    [SerializeField] private float moveSpeedOnGround = 5f;
    public float MoveSpeedOnGround => moveSpeedOnGround;

    [SerializeField] private float moveSpeedInAir = 3f;
    public float MoveSpeedInAir => moveSpeedInAir;
    
    [SerializeField] private float _accelerationSpeed = 30f;
    public float AccelerationSpeed => _accelerationSpeed;

    [SerializeField] private float decelerationSpeed = 25f;
    public float DecelerationSpeed => decelerationSpeed;

    [SerializeField] private float maxFallSpeed = 15f;
    public float MaxFallSpeed => maxFallSpeed;

    [SerializeField] private float jumpForce = 5f;
    public float JumpForce => jumpForce;
}
