using UnityEngine;

public class CharacterAnimation : MonoBehaviour
{   
    // --- References ---
    [SerializeField] private Animator _animator;
    [SerializeField] private SpriteRenderer _body;
    [SerializeField] private SpriteRenderer _mask;

    private static readonly int IdleState = Animator.StringToHash("Ch_Body_Idle_01");
    private static readonly int WalkState = Animator.StringToHash("Ch_Body_Walk_01");

    public void PlayIdle() => PlayAnimation(IdleState);
    public void PlayWalk() => PlayAnimation(WalkState);

    // --- Offsets ---
    // All offsets originally are on the right side
    [SerializeField] private Vector2 _maskOffset_Idle;
    [SerializeField] private Vector2 _maskOffset_Walk;

    private int _currentState;

    private void FlipSprites(bool facingRight)
    {
        _body.flipX = !facingRight;
        _mask.flipX = !facingRight;
    }
    
    public void AlignIdle(int direction)
    {
        if (direction == 0) return;

        Transform mask = _mask.transform;

        if (direction == 1) { mask.localPosition = _maskOffset_Idle; FlipSprites(true); }
        else {mask.localPosition = new Vector2(-_maskOffset_Idle.x, _maskOffset_Idle.y); FlipSprites(false);}
    }

    public void AlignWalk(int direction)
    {
        if (direction == 0) return;

        Transform mask = _mask.transform;

        if (direction == 1) { mask.localPosition = _maskOffset_Walk; FlipSprites(true); }
        else {mask.localPosition = new Vector2(-_maskOffset_Walk.x, _maskOffset_Walk.y); FlipSprites(false);}
    }

    public void PlayAnimation(int newStateHash)
    {
        if (_currentState == newStateHash) return; 

        _animator.Play(newStateHash, 0);
        _currentState = newStateHash;
    }
}
