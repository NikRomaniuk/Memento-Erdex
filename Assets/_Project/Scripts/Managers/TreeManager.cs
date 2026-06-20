using System.Collections.Generic;
using UnityEngine;

public class TreeManager : MonoBehaviour
{
    // --- Settings ---
    // How often (in seconds) to check chunks (timer mode)
    private const float CheckInterval = 0.1f;

    // --- References ---
    [SerializeField] private CharacterManager _character; // Character
    [SerializeField] private Transform _cameraTransform;  // Camera
    [SerializeField] private Reference_Int _cameraLoadRadius; // Load radius around camera
    [SerializeField] private Reference_Int _playerLoadRadius; // Load radius around player

    [Header("Check Mode")]
    [SerializeField] private bool _checkByTimer = true; // true = timer, false = by distance
    [SerializeField] private Observable_Vector3 _currentCameraPosition; // Camera position for distance mode
    [SerializeField] private Reference_Float _positionUpdateThreshold; // Min distance to trigger update (X/Y only)

    // --- State ---
    private float _timer;    // Countdown to next Chunk check (timer mode)
    private bool _isReady;   // True once TreeGenerator has finished generating
    private Vector3 _lastUpdatedOnPosition; // Last position where chunks were updated (distance mode)

    private void Update()
    {
        // --- Wait for generation to complete ---
        if (!_isReady)
        {
            if (TreeLoader.GenData == null) return;
            // Generation completed -> Start immediately
            _isReady = true;
            _timer = CheckInterval;
            _lastUpdatedOnPosition = _currentCameraPosition != null
                ? _currentCameraPosition.Value
                : (_cameraTransform != null ? _cameraTransform.position : Vector3.zero);
            CheckCurrentChunks();
            return;
        }

        if (_checkByTimer)
        {
            // --- Timer mode ---
            _timer -= Time.deltaTime;
            if (_timer <= 0f)
            {
                _timer = CheckInterval;
                CheckCurrentChunks();
                _lastUpdatedOnPosition = _currentCameraPosition != null
                    ? _currentCameraPosition.Value
                    : (_cameraTransform != null ? _cameraTransform.position : Vector3.zero);
            }
        }
        else
        {
            // --- Distance mode ---
            // Only update when camera moves beyond threshold
            if (_currentCameraPosition == null) return;

            Vector3 currentPos = _currentCameraPosition.Value;

            // Compare X and Y only, ignore Z
            float dist = Vector2.Distance(
                new Vector2(_lastUpdatedOnPosition.x, _lastUpdatedOnPosition.y),
                new Vector2(currentPos.x, currentPos.y)
            );

            if (dist >= _positionUpdateThreshold.Value)
            {
                CheckCurrentChunks();
                _lastUpdatedOnPosition = currentPos;
            }
        }
    }

    /// <summary>
    /// Finds the Chunk for the Camera and for the Character,
    /// merges their load ranges, and tells <see cref="TreeLoader"/> to keep them loaded
    /// </summary>
    private void CheckCurrentChunks()
    {
        // --- Validations ---
        if (_character == null || _cameraTransform == null) return;
        if (TreeLoader.GenData == null) return;

        var chunks = TreeLoader.GenData.Chunks;
        if (chunks.Count == 0) return;

        // --- Find chunk index for Camera ---
        int cameraIndex = FindChunkIndex(_cameraTransform.position.y, chunks);

        // --- Find chunk index for Player ---
        int playerIndex = FindChunkIndex(_character.transform.position.y, chunks);

        // --- Merge ranges into a single set ---
        var allDesired = new HashSet<int>();

        allDesired.UnionWith(GetDesiredRange(cameraIndex, _cameraLoadRadius.Value, chunks.Count));
        allDesired.UnionWith(GetDesiredRange(playerIndex, _playerLoadRadius.Value, chunks.Count));

        // --- Request Load ---
        TreeLoader.KeepLoaded(allDesired);
    }

    /// <summary>
    /// Forces an immediate Chunk refresh
    /// </summary>
    public void ForceRefreshChunks()
    {
        CheckCurrentChunks();
        _timer = CheckInterval; // Reset timer
        _lastUpdatedOnPosition = _currentCameraPosition != null
            ? _currentCameraPosition.Value
            : (_cameraTransform != null ? _cameraTransform.position : Vector3.zero);
    }

    /// <summary>
    /// Finds which chunk index a given Y position falls into
    /// </summary>
    private int FindChunkIndex(float y, List<ChunkGen> chunks)
    {
        // Below all bounds -> first chunk
        if (y < chunks[0].CurrentHeight) return 0;

        for (int i = 0; i < chunks.Count - 1; i++)
        {
            if (y >= chunks[i].CurrentHeight && y < chunks[i + 1].CurrentHeight)
                return i;
        }

        // Above all bounds -> last chunk
        return chunks.Count - 1;
    }

    /// <summary>
    /// Builds a set of chunk indices within <paramref name="radius"/> of <paramref name="centerIndex"/>
    /// </summary>
    private HashSet<int> GetDesiredRange(int centerIndex, int radius, int chunkCount)
    {
        int min = Mathf.Max(0, centerIndex - radius);
        int max = Mathf.Min(chunkCount - 1, centerIndex + radius);

        var range = new HashSet<int>();
        for (int i = min; i <= max; i++)
            range.Add(i);

        return range;
    }
}
