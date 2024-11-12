using System.Buffers;

namespace LiveRunnerEngine;

public class ChunkManager
{
    private readonly Chunk[] _availableChunks =
    [
        Chunks.Empty,
        Chunks.RightLaneBlockedWithObstacle,
        Chunks.CenterLaneBlockedWithObstacle,
        Chunks.LeftLaneBlockedWithObstacle,
        Chunks.LeftAndRightLaneBlockedWithObstacle,
        Chunks.PlayingCard3Variation1,
        Chunks.PlayingCard3Variation2,
        Chunks.ChicaneLeft,
        Chunks.ChicaneRight,
    ];

    private readonly int _laneCount;
    private readonly SpriteKind[] _lastChunkRow;

    public ChunkManager(int laneCount)
    {
        _laneCount = laneCount;
        _lastChunkRow = new SpriteKind[laneCount];
    }

    public Chunk PickNextChunk()
    {
        var nextChunks = ArrayPool<Chunk>.Shared.Rent(_availableChunks.Length);
        try
        {
            var chunksThatFit = 0;
            for (var i = 0; i < _availableChunks.Length; i++)
            {
                var chunk = _availableChunks[i];

                // add the chunk if it can fit
                if (CanChunkFit(chunk))
                    nextChunks[chunksThatFit] = chunk;

                chunksThatFit++;
            }

            // pick a random one from the list of chunks that fit
            var nextChunk = nextChunks[Random.Shared.Next(chunksThatFit)];

            // track the last sprite in each lane
            for (var lane = 0; lane < _laneCount; lane++)
            {
                _lastChunkRow[lane] = nextChunk.Sprites[lane][nextChunk.Length - 1];
            }

            return nextChunk;
        }
        finally
        {
            ArrayPool<Chunk>.Shared.Return(nextChunks);
        }
    }

    public bool CanChunkFit(Chunk chunk) =>
        CanChunkFit(_lastChunkRow, chunk);

    public static bool CanChunkFit(Chunk lastChunk, Chunk nextChunk)
    {
        var lastRow = ArrayPool<SpriteKind>.Shared.Rent(lastChunk.Lanes);
        try
        {
            // copy the last row of the last chunk
            for (var lane = 0; lane < lastChunk.Lanes; lane++)
            {
                lastRow[lane] = lastChunk.Sprites[lane][lastChunk.Length - 1];
            }

            return CanChunkFit(lastRow.AsSpan(0, lastChunk.Lanes), nextChunk);
        }
        finally
        {
            ArrayPool<SpriteKind>.Shared.Return(lastRow);
        }
    }

    public static bool CanChunkFit(ReadOnlySpan<SpriteKind> lastChunk, Chunk nextChunk)
    {
        // for each lane, check to see if it was empty and
        // if it was, then we need to make sure that at least one of the lanes
        // in the the new chunk also has an empty lane in the same spot
        for (var laneNumber = 0; laneNumber < lastChunk.Length; laneNumber++)
        {
            var lastSprite = lastChunk[laneNumber];
            var newSprite = nextChunk.Sprites[laneNumber][0];

            if (lastSprite == SpriteKind.None && newSprite == lastSprite)
            {
                return true;
            }
        }

        // there were no matching open lanes, so we can't use this chunk
        return false;
    }
}
