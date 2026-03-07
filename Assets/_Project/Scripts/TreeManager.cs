using UnityEngine;

public class TreeManager : MonoBehaviour
{
    // --- Settings ---
    // How often (in seconds) to check the Character's Chunk
    private const float CheckInterval = 0.1f;

    // --- References ---
    [SerializeField] private CharacterManager _character; // Character

    // --- State ---
    private float _timer;    // Countdown to next Chunk check
    private bool _isReady;   // True once TreeGenerator has finished generating

    private void Update()
    {
        // --- Wait for generation to complete ---
        if (!_isReady)
        {
            if (TreeLoader.GenData == null) return;
            // Generation completed -> Start immediately
            _isReady = true;
            _timer = CheckInterval;
            CheckCurrentChunk();
            return;
        }

        // --- Tick Timer ---
        _timer -= Time.deltaTime;
        if (_timer <= 0f)
        {
            _timer = CheckInterval; // Reset timer
            CheckCurrentChunk();
        }
    }

    /// <summary>
    /// Finds the Chunk the Character is currently inside and tells <see cref="TreeLoader"/> to keep it loaded
    /// </summary>
    private void CheckCurrentChunk()
    {
        // --- Validations ---
        if (_character == null) return;
        if (TreeLoader.GenData == null) return;

        var chunks = TreeLoader.GenData.Chunks;
        if (chunks.Count == 0) return;

        // --- Find current Chunk ---
        float y = _character.transform.position.y;
        int foundIndex = chunks.Count - 1; // Default to last Chunk if above all bounds

        for (int i = 0; i < chunks.Count - 1; i++)
        {
            if (y < chunks[0].CurrentHeight) // Below all bounds -> Use first Chunk
            {
                foundIndex = 0;
                break;
            }

            if (y >= chunks[i].CurrentHeight && y < chunks[i + 1].CurrentHeight)
            {
                foundIndex = i;
                break;
            }
        }
        // --- Request Load ---
        TreeLoader.KeepLoaded(foundIndex);
    }
}
