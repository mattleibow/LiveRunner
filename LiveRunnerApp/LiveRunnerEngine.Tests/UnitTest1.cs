namespace LiveRunnerEngine.Tests;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        var lastChunk = Chunks.CenterLaneBlockedWithObstacle;
        var nextChunk = Chunks.LeftAndRightLaneBlockedWithObstacle;

        var canFit = ChunkManager.CanChunkFit(lastChunk, nextChunk);

        Assert.False(canFit);
    }

    [Fact]
    public void Test2()
    {
        var lastChunk = Chunks.LeftAndRightLaneBlockedWithObstacle;
        var nextChunk = Chunks.ChicaneLeft;

        var canFit = ChunkManager.CanChunkFit(lastChunk, nextChunk);

        Assert.False(canFit);
    }
}